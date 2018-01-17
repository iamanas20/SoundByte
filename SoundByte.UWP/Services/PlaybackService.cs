using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using JetBrains.Annotations;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Extensions;
using YoutubeExplode;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     UWP implementation of the Playback Service
    /// </summary>
    public partial class PlaybackService : IPlaybackService
    {
        #region Delegates
        public delegate void TrackChangedEventHandler(BaseTrack newTrack);

        public delegate void StateChangedEventHandler(MediaPlaybackState mediaPlaybackState);
        #endregion

        #region Events
        /// <summary>
        ///     This event is fired when the current track changes.
        /// </summary>
        public event TrackChangedEventHandler OnTrackChange;

        /// <summary>
        ///     This event is fired when the current playback state changes
        ///     (e.g paused, playing, stopped, opening etc.)
        /// </summary>
        public event StateChangedEventHandler OnStateChange;
        #endregion

        #region Private Variables
        /// <summary>
        ///     Used for working with YouTube video streams. This is a shared
        ///     instance to increase performance.
        /// </summary>
        private YoutubeClient _youTubeClient;

        /// <summary>
        ///     Shared media player used throughout the app.
        /// </summary>
        private MediaPlayer _mediaPlayer;

        /// <summary>
        ///     The currently playing track
        /// </summary>
        [CanBeNull]
        private BaseTrack _currentTrack;

        /// <summary>
        ///     Used for timeline activity
        /// </summary>
        private UserActivityChannel _userActivityChannel;

        /// <summary>
        ///     Media Playback List that allows queuing of songs and 
        ///     gapless playback.
        /// </summary>
        private MediaPlaybackList MediaPlaybackList => _mediaPlayer.Source as MediaPlaybackList;

        /// <summary>
        ///     The current playlist token (next items in the list)
        /// </summary>
        private string _playlistToken;

        /// <summary>
        ///     The source of items to load.
        /// </summary>
        private ISource<BaseTrack> _playlistSource;
        #endregion

        #region Constructor
        /// <summary>
        /// Setup the playback service class for use.
        /// </summary>
        public PlaybackService()
        {
            // Only keep 5 items open and do not auto repeat
            // as we will be loading more items once we reach the
            // end of a list (or starting over if in playlist)
            var mediaPlaybackList = new MediaPlaybackList
            {
                MaxPlayedItemsToKeepOpen = 2,
                AutoRepeatEnabled = false
            };

            // Create the media player and disable auto play
            // as we are going to use a playback list. Set the
            // source to the media playback list. Auto play is true so if
            // we reach the end of a playlist (or source) start from the beginning.
            _mediaPlayer = new MediaPlayer
            {
                AutoPlay = true,
                Source = mediaPlaybackList
            };

            // Create the youtube client used to parse YouTube streams.
            _youTubeClient = new YoutubeClient();

            // Assign event handlers
            MediaPlaybackList.CurrentItemChanged += MediaPlaybackListOnCurrentItemChanged;
            _mediaPlayer.CurrentStateChanged += MediaPlayerOnCurrentStateChanged;
            _mediaPlayer.MediaFailed += (sender, args) =>
            {
               Debug.WriteLine(args.ErrorMessage);
            };

            _userActivityChannel = UserActivityChannel.GetDefault();

        }

        private void MediaPlayerOnCurrentStateChanged(MediaPlayer sender, object args)
        {
            OnStateChange?.Invoke(sender.PlaybackSession.PlaybackState);
        }

        #endregion

        #region Private Event Handlers
        /// <summary>
        ///     Occurs when a current media playback item changes.
        /// </summary>
        private async void MediaPlaybackListOnCurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            var track = args.NewItem?.Source.AsBaseTrack();

            // If there is no new item, don't do anything
            if (track == null)
                return;

            // Invoke the track change method
            OnTrackChange?.Invoke(track);

            Debug.WriteLine(JsonConvert.SerializeObject(track));

            await Task.Run(async () =>
            {
                string currentUsageLimit;
                var memoryUsage = MemoryManager.AppMemoryUsage / 1024 / 1024;

                if (memoryUsage > 512)
                {
                    currentUsageLimit = "More than 512MB";
                }
                else if (memoryUsage > 256)
                {
                    currentUsageLimit = "More than 256MB";
                }
                else if (memoryUsage > 128)
                {
                    currentUsageLimit = "More than 128MB";
                }
                else
                {
                    currentUsageLimit = "Less than 128MB";
                }

                App.Telemetry.TrackEvent("Current Song Change", new Dictionary<string, string>
                {
                    { "Current Usage", currentUsageLimit },
                    { "Free", SystemInformation.AvailableMemory.ToString(CultureInfo.InvariantCulture) },
                    { "Track Type", track.ServiceType.ToString() ?? "Null" },
                    { "Device", SystemInformation.DeviceFamily },
                    { "Current Version / First Version", SystemInformation.FirstVersionInstalled.ToFormattedString() + "/" + SystemInformation.ApplicationVersion.ToFormattedString()},
                });

                try
                {
                    // Only perform logic if soundbyte account is connected
                    // and the track type is not a local track
                    if (SoundByteService.Current.IsSoundByteAccountConnected 
                        && track.ServiceType != ServiceType.Local)
                    {
                        await SoundByteService.Current.PostItemAsync(ServiceType.SoundByte, "history", track);
                    }
                }
                catch (Exception ex)
                {
                    var i = 0;
                }
            });

            

            // Find the index of this item and see if we are near the end
            var currentIndex = MediaPlaybackList.ShuffleEnabled 
                ? MediaPlaybackList.ShuffledItems.ToList().IndexOf(args.NewItem) 
                : MediaPlaybackList.Items.IndexOf(args.NewItem);

            var maxIndex = MediaPlaybackList.ShuffleEnabled 
                ? MediaPlaybackList.ShuffledItems.Count - 1 
                : MediaPlaybackList.Items.Count - 1;

            // When we are three items from the end, load more items
            if (currentIndex >= maxIndex - 3)
            {
                var newItems = await _playlistSource.GetItemsAsync(50, _playlistToken);
                _playlistToken = newItems.Token;

                if (newItems.IsSuccess)
                {
                    // Loop through all the tracks and add them to the playlist
                    foreach (var newTrack in newItems.Items)
                    {
                        try
                        {
                            BuildMediaItem(newTrack);
                        }
                        catch (Exception e)
                        {
                            App.Telemetry.TrackEvent("Playback Item Addition Failed", new Dictionary<string, string>
                            {
                                { "TrackID", newTrack.TrackId },
                                { "TrackService", newTrack.ServiceType.ToString() },
                                { "ErrorMessage", e.Message }
                            });
                        }
                    }
                }   
            }
        }
        #endregion

        #region Track Controls

        #region Getters
        /// <summary>
        ///     Has the current playlist been shuffled.
        /// </summary>
        public bool IsPlaylistShuffled => MediaPlaybackList.ShuffleEnabled;

        /// <summary>
        ///     Is the current track muted
        /// </summary>
        public bool IsTrackMuted => _mediaPlayer.IsMuted;

        /// <summary>
        ///     Will the current track repeat when finished.
        /// </summary>
        public bool IsTrackRepeating => _mediaPlayer.IsLoopingEnabled;

        /// <summary>
        ///     Volume of the current playing track.
        /// </summary>
        public double TrackVolume => _mediaPlayer.Volume;

        public MediaPlaybackState CurrentPlaybackState => _mediaPlayer.PlaybackSession.PlaybackState;
        #endregion

        /// <summary>
        ///     Shuffle the playlist
        /// </summary>
        /// <param name="shuffle">True to shuffle, false to not.</param>
        public async void ShufflePlaylist(bool shuffle)
        {
            // Start a random track
            await StartRandomTrackAsync();

            // Track event
            App.Telemetry.TrackEvent("Shuffle Playlist", new Dictionary<string, string>
            {
                { "IsShuffled", shuffle.ToString() }
            });
        }

        public void MuteTrack(bool mute)
        {
            _mediaPlayer.IsMuted = mute;
        }

        public void SetTrackVolume(double volume)
        {
            _mediaPlayer.Volume = volume;
        }

        public void SetTrackPosition(TimeSpan value) => _mediaPlayer.PlaybackSession.Position = value;

        public TimeSpan GetTrackPosition() => _mediaPlayer.PlaybackSession.Position;

        public TimeSpan GetTrackDuration() => _mediaPlayer.PlaybackSession.NaturalDuration;

        public void RepeatTrack(bool repeat)
        {
            _mediaPlayer.IsLoopingEnabled = repeat;
        }

        /// <summary>
        ///     Move to the next item.
        /// </summary>
        public void NextTrack()
        {
            _mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;
            MediaPlaybackList.MoveNext();
        }

        /// <summary>
        ///     Move to the previous item
        /// </summary>
        public void PreviousTrack()
        {
            _mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;
            MediaPlaybackList.MovePrevious();
        }

        /// <summary>
        ///     Pause the current track
        /// </summary>
        public void PauseTrack()
        {
            if (_mediaPlayer.CanPause)
                _mediaPlayer.Pause();

            if (SoundByteService.Current.IsSoundByteAccountConnected)
            {
                // TODO: Remote features.
            }
        }

        /// <summary>
        ///     Play the current track
        /// </summary>
        public void PlayTrack()
        {
            _mediaPlayer.Play();

            if (SoundByteService.Current.IsSoundByteAccountConnected)
            {
                // TODO: Remote features.
            }
        }
        #endregion

        public async Task StartTrackAsync(BaseTrack trackToPlay = null)
        {
            MediaPlaybackList.ShuffleEnabled = false;

            _mediaPlayer.Pause();

            if (trackToPlay == null)
            {
                _mediaPlayer.Play();
                return;
            }

            var keepTrying = 0;

            while (keepTrying < 50)
            {
                try
                {
                    // find the index of the track in the playlist
                    var index = MediaPlaybackList.Items.ToList()
                        .FindIndex(item => item.Source.AsBaseTrack().TrackId ==
                                           trackToPlay.TrackId);

                    if (index == -1)
                    {
                        await Task.Delay(50);
                        keepTrying++;
                        continue;
                    }

                    // Move to the track
                    MediaPlaybackList.MoveTo((uint)index);

                    // Begin playing
                    _mediaPlayer.Play();

                    return;
                }
                catch (Exception)
                {
                    keepTrying++;
                    await Task.Delay(200);
                }
            }
            // Just play the first item
            _mediaPlayer.Play();
        }

        public async Task StartRandomTrackAsync()
        {
            var playItemsCount = MediaPlaybackList.Items.Count;

            // Get a random index
            var index = new Random().Next(0, playItemsCount - 1);

            // Start the track
            await StartTrackAsync(MediaPlaybackList.Items.ElementAt(index)?.Source.AsBaseTrack());

            // Call this afterwards (as the above method unshuffles items)
            MediaPlaybackList.ShuffleEnabled = true;
        }

        public BaseTrack GetCurrentTrack()
        {
            return MediaPlaybackList?.CurrentItem?.Source?.AsBaseTrack();
        }

        public async Task<PlaybackInitilizeResponse> InitilizePlaylistAsync<T>(IEnumerable<BaseTrack> playlist = null,
            string token = null) where T : ISource<BaseTrack>
        {
            return await InitilizePlaylistAsync(Activator.CreateInstance<T>(), playlist, token);
        }

        public async Task<PlaybackInitilizeResponse> InitilizePlaylistAsync<T>(T model, IEnumerable<BaseTrack> playlist = null, 
            string token = null) where T : ISource<BaseTrack>
        {
            _playlistSource = model;
            _playlistToken = token;

            // We are changing media
            _mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;

            // Pause the media player and clear the currenst playlist
            _mediaPlayer.Pause();
            MediaPlaybackList.Items.Clear();

            // If the playlist does not exist, or was not passed in, we
            // need to load the first 50 items.
            if (playlist == null)
            {
                try
                {
                    // Get (up to) 50 items and update the token
                    var responseItems = await _playlistSource.GetItemsAsync(50, _playlistToken);
                    _playlistToken = responseItems.Token;
                    playlist = responseItems.Items;
                }
                catch (Exception e)
                {
                    return new PlaybackInitilizeResponse(false, "Error Loading Playlist: " + e.Message);
                }
            }

            // Loop through all the tracks and add them to the playlist
            foreach (var track in playlist)
            {
                try
                {
                    BuildMediaItem(track);
                }
                catch (Exception e)
                {
                    App.Telemetry.TrackEvent("Playback Item Addition Failed", new Dictionary<string, string>
                    {
                        { "TrackID", track.TrackId },
                        { "TrackService", track.ServiceType.ToString() },
                        { "ErrorMessage", e.Message }
                    });
                }
            }

            // Everything loaded fine
            return new PlaybackInitilizeResponse();
        }

        /// <summary>
        ///     Build a media item and add it to the list
        /// </summary>
        /// <param name="track">Track to build into a media item</param>
        private void BuildMediaItem(BaseTrack track)
        {
            // Create a media binding for later (this is used to
            // load the track streams as we need them).
            var binder = new MediaBinder { Token = track.TrackId };
            binder.Binding += BindMediaSource;

            // Create the source, bind track metadata and use it to
            // create a playback item
            var source = MediaSource.CreateFromMediaBinder(binder);
            var mediaPlaybackItem = new MediaPlaybackItem(track.AsMediaSource(source));

            // Apply display properties to this item
            var displayProperties = mediaPlaybackItem.GetDisplayProperties();
            displayProperties.Type = MediaPlaybackType.Music;
            displayProperties.MusicProperties.Title = track.Title;
            displayProperties.MusicProperties.Artist = track.User.Username;
            displayProperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(ArtworkConverter.ConvertObjectToImage(track)));

            // Apply the properties
            mediaPlaybackItem.ApplyDisplayProperties(displayProperties);

            // Add this item to the list
            MediaPlaybackList.Items.Add(mediaPlaybackItem);
        }

        #region Media Binding
        private async void BindMediaSource(MediaBinder sender, MediaBindingEventArgs args)
        {
            var deferal = args.GetDeferral();

            // Get the track data
            var track = MediaPlaybackList.Items.ToList()
                .FirstOrDefault(x => x.Source.AsBaseTrack().TrackId == args.MediaBinder.Token)
                ?.Source?.AsBaseTrack();

            // Only run if the track exists
            if (track != null)
            {
                // Get the audio stream url for this track
                var audioStreamUri = await track.GetAudioStreamAsync(_youTubeClient);

                // If we are live and youtube, we get an adaptive stream url
                if (track.ServiceType == ServiceType.YouTube && track.IsLive)
                {
                    var source = await AdaptiveMediaSource.CreateFromUriAsync(audioStreamUri);
                    if (source.Status == AdaptiveMediaSourceCreationStatus.Success)
                    {
                        args.SetAdaptiveMediaSource(source.MediaSource);
                    }
                }
                else if (track.ServiceType == ServiceType.Local)
                {
                    var file = track.CustomProperties["File"] as StorageFile;

                    args.SetStorageFile(file);
                }
                else if (track.ServiceType == ServiceType.ITunesPodcast)
                {
                    args.SetUri(new Uri(track.AudioStreamUrl));
                }
                else
                {
                    // Set generic stream url.
                    args.SetUri(audioStreamUri);
                }
            }

            deferal.Complete();

        }
        #endregion
    }

    /// <summary>
    ///     UWP implementation of the Playback Service
    /// </summary>
    public partial class PlaybackService
    {
        private static PlaybackService _instance;

        /// <summary>
        ///     Singleton instance of <see cref="PlaybackService"/>.
        /// </summary>
        public static PlaybackService Instance => _instance ?? (_instance = new PlaybackService());
    }
}
