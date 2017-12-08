/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using AudioVisualizer;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Helpers;
using SoundByte.Core.Services;
using SoundByte.UWP.DatabaseContexts;
using SoundByte.Core.Models.MediaStreams;
using SoundByte.Core.Sources;
using SoundByte.UWP.Assets;
using Windows.Foundation.Collections;
using Microsoft.EntityFrameworkCore;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     The centeral way of accessing playback within the
    ///     app, provides access the the media player and active
    ///     playlist.
    /// </summary>
    public class PlaybackService : INotifyPropertyChanged
    {
        #region Delegates
        public delegate void CurrentTrackChangedEventHandler(BaseTrack newTrack);
        #endregion

        #region Events
        /// <summary>
        /// This event is fired when the current track changes.
        /// </summary>
        public event CurrentTrackChangedEventHandler OnCurrentTrackChanged;

        public event EventHandler<IVisualizationSource> VisualizationSourceChanged;
        #endregion

        #region Class Variables / Getters and Setters

        // Playlist Object
        private MediaPlaybackList _playbackList;

        public PlaybackSource VisualizationSource { get; set; }

        /// The audio-video sync timer is used to sync YouTube videos
        /// to the background audio. This has to run a little faster for
        /// a smoother expirence.
        private readonly DispatcherTimer _audioVideoSyncTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(75)
        };

        /// <summary>
        /// The current playing track
        /// </summary>
        [CanBeNull]
        public BaseTrack CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (_currentTrack == value)
                    return;

                _currentTrack = value;
                UpdateProperty();
            }
        }
        private BaseTrack _currentTrack;

        /// <summary>
        ///     The amount of time spent listening to the track
        /// </summary>
        public string TimeListened
        {
            get => _timeListened;
            set
            {
                if (_timeListened == value)
                    return;

                _timeListened = value;
                UpdateProperty();
            }
        }
        private string _timeListened = "00:00";

        /// <summary>
        ///     The amount of time remaining
        /// </summary>
        public string TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                if (_timeRemaining == value)
                    return;

                _timeRemaining = value;
                UpdateProperty();
            }
        }
        private string _timeRemaining = "-00:00";

        /// <summary>
        ///     The current slider value
        /// </summary>
        public double CurrentTimeValue
        {
            get => _currentTimeValue;
            set
            {
                _currentTimeValue = value;
                UpdateProperty();
            }
        }
        private double _currentTimeValue;

        /// <summary>
        ///     The max slider value
        /// </summary>
        public double MaxTimeValue
        {
            get => _maxTimeValue;
            private set
            {
                _maxTimeValue = value;
                UpdateProperty();
            }
        }
        private double _maxTimeValue = 100;

        /// <summary>
        ///     The current text for the volume icon
        /// </summary>
        public string VolumeIcon
        {
            get => _volumeIcon;
            private set
            {
                if (_volumeIcon == value)
                    return;

                _volumeIcon = value;
                UpdateProperty();
            }
        }
        private string _volumeIcon = "\uE767";

        /// <summary>
        ///     The content on the play_pause button
        /// </summary>
        public string PlayButtonContent
        {
            get => _playButtonContent;
            set
            {
                if (_playButtonContent == value)
                    return;

                _playButtonContent = value;
                UpdateProperty();
            }
        }
        private string _playButtonContent = "\uE769";

        /// <summary>
        ///     The data model of the active playlist.
        /// </summary>
        public ObservableCollection<BaseTrack> Playlist
        {
            get => _playlist;
            private set
            {
                _playlist = value;
                UpdateProperty();
            }
        }
        private ObservableCollection<BaseTrack> _playlist = new ObservableCollection<BaseTrack>();

        /// <summary>
        ///     The current value of the volume slider
        /// </summary>
        public double MediaVolume
        {
            get => Player.Volume * 100;
            set
            {
                // Set the volume
                Player.Volume = value / 100;

                // Update the UI
                if ((int)value == 0)
                {
                    Player.IsMuted = true;
                    VolumeIcon = "\uE74F";
                }
                else if (value < 25)
                {
                    Player.IsMuted = false;
                    VolumeIcon = "\uE992";
                }
                else if (value < 50)
                {
                    Player.IsMuted = false;
                    VolumeIcon = "\uE993";
                }
                else if (value < 75)
                {
                    Player.IsMuted = false;
                    VolumeIcon = "\uE994";
                }
                else
                {
                    Player.IsMuted = false;
                    VolumeIcon = "\uE767";
                }

                UpdateProperty();
            }
        }

        /// <summary>
        ///     Get the current list of items to be played back
        /// </summary>
        public MediaPlaybackList PlaybackList => Player.Source as MediaPlaybackList;

        /// <summary>
        ///     This application only requires a single shared MediaPlayer
        ///     that all pages have access to.
        /// </summary>
        public MediaPlayer Player { get; }

        /// <summary>
        ///     Are tracks shuffled
        /// </summary>
        public bool IsShuffleEnabled
        {
            get => PlaybackList?.ShuffleEnabled ?? false;
            set
            {
                if (PlaybackList == null) return;

                if (PlaybackList.ShuffleEnabled == value)
                    return;

                PlaybackList.ShuffleEnabled = value;
                UpdateProperty();
            }
        }

        /// <summary>
        ///     Is the song going to repeat when finished
        /// </summary>
        public bool IsRepeatEnabled
        {
            get => Player?.IsLoopingEnabled ?? false;
            set
            {
                if (Player == null) return;

                if (Player.IsLoopingEnabled == value)
                    return;

                Player.IsLoopingEnabled = value;
                UpdateProperty();
            }
        }

        #endregion

      

        #region Service Setup
        private static readonly Lazy<PlaybackService> InstanceHolder =
            new Lazy<PlaybackService>(() => new PlaybackService());

        public static PlaybackService Instance => InstanceHolder.Value;

        /// <summary>
        /// Setup the playback service class
        /// </summary>
        private PlaybackService()
        {
            // Create the player instance
            Player = new MediaPlayer { AutoPlay = false };

            Player.PlaybackSession.PlaybackStateChanged += PlaybackSessionStateChanged;

            // Create the new playback list (with autorepeat enabled)
            _playbackList = new MediaPlaybackList
            {
                AutoRepeatEnabled = true,
                MaxPlayedItemsToKeepOpen = 20
            };

            // Subscribe to the item change event
            _playbackList.CurrentItemChanged += CurrentItemChanged;

            // Set the playback list
            Player.Source = _playbackList;

            // The page timer is used to update things like current position, time
            // remaining time etc.
            var pageTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            // Setup the tick event
            pageTimer.Tick += PlayingSliderUpdate;
            _audioVideoSyncTimer.Tick += SyncAudioVideo;

            // If the timer is ready, start it
            if (!pageTimer.IsEnabled)
                pageTimer.Start();
        }
        #endregion

        #region Property Changed Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Playback Control
        /// <summary>
        ///     Toggle if the current song should repeat.
        /// </summary>
        public void ToggleRepeat()
        {
            IsRepeatEnabled = !IsRepeatEnabled;

            TelemetryService.Instance.TrackEvent("Toggle Repeat");
        }

        /// <summary>
        ///     Toggle if we should shuffle the items
        /// </summary>
        public void ToggleShuffle()
        {
            IsShuffleEnabled = !IsShuffleEnabled;

            TelemetryService.Instance.TrackEvent("Toggle Shuffle");
        }

        /// <summary>
        ///     Toggle the media mute
        /// </summary>
        public void ToggleMute()
        {
            // Toggle the mute
            Player.IsMuted = !Player.IsMuted;

            // Update the UI
            VolumeIcon = Player.IsMuted ? "\uE74F" : "\uE767";
        }

        /// <summary>
        ///     Toggles the state between the track playing
        ///     and not playing
        /// </summary>
        public void ChangePlaybackState()
        {
            // Get the current state of the track
            var currentState = Player.PlaybackSession.PlaybackState;

            // If the track is currently paused
            if (currentState == MediaPlaybackState.Paused)
            {
                UpdateNormalTiles();
                // Play the track
                Player.Play();
            }

            // If the track is currently playing
            if (currentState == MediaPlaybackState.Playing)
            {
                UpdatePausedTile();
                // Pause the track
                Player.Pause();
            }
        }

        /// <summary>
        ///     Go forward one track
        /// </summary>
        public async void SkipNext()
        {
            // Tell the controls that we are changing song
            Player.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;

            // Move to the next item
            await Task.Run(() => { _playbackList.MoveNext(); });
        }

        /// <summary>
        ///     Go backwards one track
        /// </summary>
        public async void SkipPrevious()
        {
            // Tell the controls that we are changing song
            Player.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;

            // Move to the previous item
            await Task.Run(() => { _playbackList.MovePrevious(); });
        }
        #endregion

        #region Timer Event Methods
        /// <summary>
        ///     Timer that keeps YouTube video and audio in sync
        /// </summary>
        private void SyncAudioVideo(object sender, object e)
        {
            // Only run this method if there is a track and it's a 
            // youtube track
            if (CurrentTrack == null || CurrentTrack.ServiceType != ServiceType.YouTube)
                return;

            // Don't run in the backround
            if (DeviceHelper.IsBackground)
                return;

            // Only call the following if the player exists, is playing
            // and the time is greater then 0.
            if (Player == null ||
                Player.PlaybackSession.PlaybackState != MediaPlaybackState.Playing ||
                Player.PlaybackSession.Position.Milliseconds <= 0)
                return;

            var overlay = App.CurrentFrame?.FindName("VideoOverlay") as MediaElement;
            if (overlay == null) return;

            var difference = overlay.Position - Player.PlaybackSession.Position;

            if (Math.Abs(difference.TotalMilliseconds) >= 1000)
            {
                overlay.PlaybackRate = 1;
                overlay.Position = Player.PlaybackSession.Position;
                System.Diagnostics.Debug.WriteLine("OUT OF SYNC: SKIPPING (>= 1000ms)");
            }
            else if (Math.Abs(difference.TotalMilliseconds) >= 500)
            {
                overlay.PlaybackRate = difference.TotalMilliseconds > 0 ? 0.25 : 1.75;
                System.Diagnostics.Debug.WriteLine("OUT OF SYNC: CHANGE PLAYBACK RATE (>= 500ms)");
            }
            else if (Math.Abs(difference.TotalMilliseconds) >= 250)
            {
                overlay.PlaybackRate = difference.TotalMilliseconds > 0 ? 0.5 : 1.5;
                System.Diagnostics.Debug.WriteLine("OUT OF SYNC: CHANGE PLAYBACK RATE (>= 250ms)");
            }
            else if (Math.Abs(difference.TotalMilliseconds) >= 100)
            {
                overlay.PlaybackRate = difference.TotalMilliseconds > 0 ? 0.75 : 1.25;
                System.Diagnostics.Debug.WriteLine("OUT OF SYNC: CHANGE PLAYBACK RATE (>= 100ms)");
            }
            else
            {
                overlay.PlaybackRate = 1;
            }
        }

        /// <summary>
        ///     Timer method that is run to make sure the UI is kept up to date
        /// </summary>
        private void PlayingSliderUpdate(object sender, object e)
        {
            if (DeviceHelper.IsBackground)
                return;

            // Only call the following if the player exists, is playing
            // and the time is greater then 0.
            if (Player == null ||
                Player.PlaybackSession.PlaybackState != MediaPlaybackState.Playing ||
                Player.PlaybackSession.Position.Milliseconds <= 0)
                return;

            if (CurrentTrack == null)
                return;

            if (CurrentTrack.IsLive)
            {
                // Set the current time value
                CurrentTimeValue = 1;

                // Set the time listened text
                TimeListened = "LIVE";

                // Set the time remaining text
                TimeRemaining = "LIVE";

                // Set the maximum value
                MaxTimeValue = 1;
            }
            else
            {
                // Set the current time value
                CurrentTimeValue = Player.PlaybackSession.Position.TotalSeconds;

                // Get the remaining time for the track
                var remainingTime = Player.PlaybackSession.NaturalDuration.Subtract(Player.PlaybackSession.Position);

                // Set the time listened text
                TimeListened = NumberFormatHelper.FormatTimeString(Player.PlaybackSession.Position.TotalMilliseconds);

                // Set the time remaining text
                TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(remainingTime.TotalMilliseconds);

                // Set the maximum value
                MaxTimeValue = Player.PlaybackSession.NaturalDuration.TotalSeconds;
            }  
        }
        #endregion

        public MediaPlaybackItem CreateMediaPlaybackItem(BaseTrack track)
        {
            if (track == null)
                return null;

            try
            {
                var binder = new MediaBinder
                {
                    Token = track.Id
                };
                binder.Binding += Binder_Binding;

                var source = MediaSource.CreateFromMediaBinder(binder);

                // So we can access the item later
                source.CustomProperties["SoundByteItem.ID"] = track.Id;
                
                // Create a configurable playback item backed by the media source
                var playbackItem = new MediaPlaybackItem(source);

                // Populate display properties for the item that will be used
                // to automatically update SystemMediaTransportControls when
                // the item is playing.
                var displayProperties = playbackItem.GetDisplayProperties();
                displayProperties.Type = MediaPlaybackType.Music;
                displayProperties.MusicProperties.Title = track.Title;
                displayProperties.MusicProperties.Artist = track.User.Username;
                displayProperties.Thumbnail =
                    RandomAccessStreamReference.CreateFromUri(
                        new Uri(ArtworkConverter.ConvertObjectToImage(track)));

                // Apply the properties
                playbackItem.ApplyDisplayProperties(displayProperties);

                // Add the item to the required lists
                return playbackItem;
            }
            catch (Exception)
            {
                TelemetryService.Instance.TrackEvent("Could not add Playback Item",
                    new Dictionary<string, string>
                    {
                        {"track_id", track?.Id}
                    });
                return null;
            }
        }

        private async void Binder_Binding(MediaBinder sender, MediaBindingEventArgs args)
        {
            // We are performing
            var defferal = args.GetDeferral();

            await App.SetLoadingAsync(true);

            try
            {
                var track = Playlist.FirstOrDefault(x => x.Id == args.MediaBinder.Token);

                if (track == null)
                    return;

                switch (track.ServiceType)
                {
                    case ServiceType.Fanburst:
                        args.SetUri(new Uri("https://api.fanburst.com/tracks/" + track.Id + "/stream?client_id=" + AppKeys.FanburstClientId));
                        break;
                    case ServiceType.SoundCloud:
                    case ServiceType.SoundCloudV2:
                        var key = await GetCorrectApiKey(track);

                        args.SetUri(new Uri("https://api.soundcloud.com/tracks/" + track.Id + "/stream?client_id=" + key));
                        break;
                    case ServiceType.YouTube:
                        var video = await new YoutubeClient().GetVideoInfoAsync(track.Id);

                        // Add missing details
                        track.Duration = video.Duration;
                        track.ViewCount = video.ViewCount;
                        track.ArtworkUrl = video.ImageHighResUrl;
                        track.AudioStreamUrl =
                            video.AudioStreams.OrderBy(q => q.AudioEncoding).Last()?.Url;

                        // 720p is max quality we want
                        var wantedQuality =
                            video.VideoStreams
                                .FirstOrDefault(x => x.VideoQuality == VideoQuality.High720)?.Url;

                        // If quality is not there, just get the highest (480p for example).
                        if (string.IsNullOrEmpty(wantedQuality))
                            wantedQuality = video.VideoStreams.OrderBy(s => s.VideoQuality).Last()?.Url;

                        track.VideoStreamUrl = wantedQuality;

                        if (track.IsLive)
                        {
                            var source = await AdaptiveMediaSource.CreateFromUriAsync(new Uri(video.AdaptiveLiveStreamUrl));
                            if (source.Status == AdaptiveMediaSourceCreationStatus.Success)
                            {
                                track.VideoStreamUrl = video.AdaptiveLiveStreamUrl;
                                args.SetAdaptiveMediaSource(source.MediaSource);
                                break;
                            }
                        }

                        args.SetUri(new Uri(track.AudioStreamUrl));
                        break;
                    case ServiceType.ITunesPodcast:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                // ignored
            }

            await App.SetLoadingAsync(false);

            defferal.Complete();
        }

        public class PlaybackResponse
        {
            public PlaybackResponse(bool _success, string _messsage)
            {
                Success = _success;
                Message = _messsage;
            }

            public string Message { get; set; }
            public bool Success { get; set; }
        }

        #region Setup/Start Media Playback Methods
        /// <summary>
        /// This method starts media playback but using a playlist instead
        /// of a model. The main disavantage of this approach is that you don't
        /// get free track loading (when nearing the end of a playlist).
        /// </summary>
        /// <param name="playlist">A list of base track items to play.</param>
        /// <param name="isShuffled">Should we shuffle the items?</param>
        /// <param name="startingItem">The track to start playing from.</param>
        /// <returns>A Tuple, if sucessful and if not, the error message.</returns>
        public async Task<PlaybackResponse> StartPlaylistMediaPlaybackAsync(IEnumerable<BaseTrack> playlist,
            bool isShuffled = false, BaseTrack startingItem = null)
        {
            // Create a dummy base track items
            var dummyBaseTrackModel = new SoundByteCollection<DummyTrackSource, BaseTrack> { Token = "eol" };

            // Add all the playlist items to this dummy
            foreach (var track in playlist)
                dummyBaseTrackModel.Add(track);

            // Call the normal method
            return await StartModelMediaPlaybackAsync(dummyBaseTrackModel, isShuffled, startingItem);
        }

        /// <summary>
        ///     Playlist a list of tracks with optional values.
        /// </summary>
        /// <param name="trackSource">A source of tracks</param>
        /// <param name="isShuffled">Should the tracks be played shuffled.</param>
        /// <param name="startingItem">What track to start with.</param>
        /// <returns></returns>
        public async Task<PlaybackResponse> StartModelMediaPlaybackAsync<TSource>(SoundByteCollection<TSource, BaseTrack> trackSource,
            bool isShuffled = false, BaseTrack startingItem = null) where TSource : ISource<BaseTrack>
        {
            // If no playlist was specified, skip
            if (trackSource == null || trackSource.Count == 0)
                return new PlaybackResponse(false,
                    "The playback list was missing or empty. This can be caused if there are not tracks avaliable (for example, you are trying to play your likes, but have not liked anything yet).\n\nAnother reason for this message is that if your playing a track from SoundCloud, SoundCloud has blocked these tracks from being played on 3rd party apps (such as SoundByte).");

            await App.SetLoadingAsync(true);

            // Pause Everything
            Player.Pause();

            // Clear the playback list
            _playbackList.Items.Clear();

            // Set the model
            Playlist = null;
            Playlist = trackSource;

            // Set the shuffle
            _playbackList.ShuffleEnabled = isShuffled;

            // Loop through all the tracks
            foreach (var track in trackSource)
            {
                var mediaPlaybackItem = CreateMediaPlaybackItem(track);

                if (mediaPlaybackItem != null)
                    _playbackList.Items.Add(mediaPlaybackItem);
            }

            // Update the controls that we are changing track
            Player.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;

            // If the track is shuffled, or no starting item is supplied, just play as usual
            if (isShuffled || startingItem == null)
            {
                Player.Play();
                return new PlaybackResponse(true, string.Empty);
            }

            var keepTrying = 0;

            while (keepTrying < 100)
                try
                {
                    // find the index of the track in the playlist
                    var index = _playbackList.Items.ToList()
                        .FindIndex(item => (string)item.Source.CustomProperties["SoundByteItem.ID"] ==
                                           startingItem.Id);

                    if (index == -1)
                    {
                        await Task.Delay(50);
                        keepTrying++;
                        continue;
                    }

                    // Move to the track
                    _playbackList.MoveTo((uint)index);
                    // Begin playing
                    Player.Play();

                    return new PlaybackResponse(true, string.Empty);
                }
                catch (Exception)
                {
                    keepTrying++;
                    await Task.Delay(200);
                }

            if (keepTrying < 50) return new PlaybackResponse(true, string.Empty);

            TelemetryService.Instance.TrackEvent("Playback Could not Start", new Dictionary<string, string>
            {
                {"track_id", startingItem.Id}
            });

            return new PlaybackResponse(false, "SoundByte could not play this track or list of tracks. Try again later.");
        }
        #endregion

        #region General Event Handlers
        /// <summary>
        ///     Called when the playback session changes
        /// </summary>
        private async void PlaybackSessionStateChanged(MediaPlaybackSession sender, object args)
        {
            // Don't run in the background if on Xbox
            if (DeviceHelper.IsBackground && DeviceHelper.IsXbox)
                return;

            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                var overlay = App.CurrentFrame?.FindName("VideoOverlay") as MediaElement;

                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.Playing:
                        await App.SetLoadingAsync(false);
                        PlayButtonContent = "\uE769";
                        overlay?.Play();
                        break;
                    case MediaPlaybackState.Buffering:
                        await App.SetLoadingAsync(true);
                        break;
                    case MediaPlaybackState.None:
                        await App.SetLoadingAsync(false);
                        PlayButtonContent = "\uE768";
                        break;
                    case MediaPlaybackState.Paused:
                        await App.SetLoadingAsync(false);
                        PlayButtonContent = "\uE768";
                        overlay?.Pause();
                        break;
                    default:
                        await App.SetLoadingAsync(false);
                        PlayButtonContent = "\uE768";
                        overlay?.Play();
                        break;
                }
            });
        }

        private async void CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            // If there is no new item, don't do anything
            if (args.NewItem == null)
                return;

            // Try get the track
            var track = Playlist.FirstOrDefault(
                x => x.Id == (string)args.NewItem.Source.CustomProperties["SoundByteItem.ID"]);

            // If the track does not exist, do nothing
            if (track == null)
                return;

            // Same track, no need to perform this logic
            if (track == CurrentTrack)
                return;

            // Invoke the track changed method
            OnCurrentTrackChanged?.Invoke(track);

            // Run all this on the UI thread
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                if (DeviceHelper.IsBackground && DeviceHelper.IsXbox)
                    return;

                // Set the new current track, updating the UI
                CurrentTrack = track;

                // Only run the sync timer when listening to a youtube music video
                if (track.ServiceType == ServiceType.YouTube)
                {
                    if (!_audioVideoSyncTimer.IsEnabled)
                        _audioVideoSyncTimer.Start();
                }
                else
                {
                    if (_audioVideoSyncTimer.IsEnabled)
                        _audioVideoSyncTimer.Stop();
                }

                if (!CurrentTrack.IsLive)
                {
                    TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(CurrentTrack.Duration.TotalMilliseconds);
                    TimeListened = "00:00";
                    CurrentTimeValue = 0;
                    MaxTimeValue = CurrentTrack.Duration.TotalSeconds;
                }
                else
                {
                    TimeRemaining = "LIVE";
                    TimeListened = "LIVE";
                    CurrentTimeValue = 1;
                    MaxTimeValue = 1;
                }
              
                // Set the last playback date
                CurrentTrack.LastPlaybackDate = DateTime.UtcNow;

                // Update the live tile
                UpdateNormalTiles();

                if (CurrentTrack?.ServiceType == ServiceType.SoundCloud)
                {
                    try
                    {
                        CurrentTrack.IsLiked = await SoundByteV3Service.Current.ExistsAsync(ServiceType.SoundCloud,
                            "/me/favorites/" + CurrentTrack.Id);
                    }
                    catch
                    {
                        CurrentTrack.IsLiked = false;
                    }

                    try
                    {
                        CurrentTrack.User = (await SoundByteV3Service.Current.GetAsync<SoundCloudUser>(ServiceType.SoundCloud, $"/users/{CurrentTrack.User.Id}")).ToBaseUser();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });

            try
            {
                using (var db = new HistoryContext())
                {
                    await db.Database.MigrateAsync();

                    var existingUser = db.Users.FirstOrDefault(x => x.Id == track.User.Id);

                    if (existingUser == null)
                        db.Users.Add(track.User);

                    await db.SaveChangesAsync();
                }

                using (var db = new HistoryContext())
                {
                    var existingUser = db.Users.FirstOrDefault(x => x.Id == track.User.Id);

                    // Get the existing track in the database (if it exists)
                    var existingTrack = db.Tracks.FirstOrDefault(x => x.Id == track.Id);

                    if (existingTrack == null)
                    {
                        track.User = existingUser;
                        db.Tracks.Add(track);
                    }
                    else
                    {
                        existingTrack.LastPlaybackDate = DateTime.UtcNow;
                    }

                    await db.SaveChangesAsync();
                }
            }
            catch 
            {
               // Ignore
            }

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

            TelemetryService.Instance.TrackEvent("Current Song Changed", new Dictionary<string, string>
            {
                { "CurrentUsage", currentUsageLimit },
                { "TrackType", track?.ServiceType.ToString() ?? "Null" },
                { "IsSoundCloudConnected", SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud).ToString() },
                { "IsFanburstConnected", SoundByteV3Service.Current.IsServiceConnected(ServiceType.Fanburst).ToString() },
                { "IsYouTubeConnected", SoundByteV3Service.Current.IsServiceConnected(ServiceType.YouTube).ToString() }
            });
        }

        /// <summary>
        ///     Called when the user adjusts the playing slider
        /// </summary>
        public async void PlayingSliderChange()
        {
            if (DeviceHelper.IsBackground)
                return;

            if (CurrentTrack == null)
                return;

            if (CurrentTrack.IsLive)
                return;

            // Set the track position
            Player.PlaybackSession.Position = TimeSpan.FromSeconds(CurrentTimeValue);

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var overlay = App.CurrentFrame.FindName("VideoOverlay") as MediaElement;
                if (overlay == null) return;

                overlay.Position = TimeSpan.FromSeconds(CurrentTimeValue);
            });
        }

        #endregion

        #region SoundCloud API Key Helpers
        private static async Task<bool> ApiCheck(string url)
        {
            try
            {
                // Create the client
                using (var client = new HttpClient(new HttpBaseProtocolFilter { AutomaticDecompression = true }))
                {
                    // No Auth for this
                    client.DefaultRequestHeaders.Authorization = null;

                    using (var webRequest = await client.GetAsync(new Uri(Uri.EscapeUriString(url))))
                    {
                        return webRequest.IsSuccessStatusCode;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static async Task<string> GetCorrectApiKey(BaseTrack track)
        {
            return await Task.Run(async () =>
            {
                // Check if we have hit the soundcloud api limit
                if (await ApiCheck(
                    $"https://api.soundcloud.com/tracks/320126814/stream?client_id={AppKeys.SoundCloudClientId}"))
                    return AppKeys.SoundCloudClientId;

                // Loop through all the backup keys
                foreach (var key in AppKeys.BackupSoundCloudPlaybackIDs)
                    if (await ApiCheck(
                        $"https://api.soundcloud.com/tracks/320126814/stream?client_id={key}"))
                        return key;

                return AppKeys.SoundCloudClientId;
            });
        }
        #endregion
       
        #region Live Tiles

        private void UpdatePausedTile()
        {
            if (CurrentTrack == null)
                return;

            if (DeviceHelper.IsXbox)
                return;

            if (!DeviceHelper.IsDesktop && !DeviceHelper.IsMobile) return;

            try
            {
                TileHelper.TileUpdater.Clear();

                var firstXml = new XmlDocument();
                firstXml.LoadXml(
                    "<tile><visual><binding template=\"TileMedium\" branding=\"nameAndLogo\"><image placement=\"peek\" src=\"" +
                    ArtworkConverter.ConvertObjectToImage(CurrentTrack) +
                    "\"/><text>Paused</text><text hint-style=\"captionsubtle\" hint-wrap=\"true\"><![CDATA[" +
                    CurrentTrack.Title.Replace("&", "&amp;") +
                    "]]></text></binding><binding template=\"TileLarge\" branding=\"nameAndLogo\"><image placement=\"peek\" src=\"" +
                    ArtworkConverter.ConvertObjectToImage(CurrentTrack) +
                    "\"/><text>Paused</text><text hint-style=\"captionsubtle\" hint-wrap=\"true\"><![CDATA[" +
                    CurrentTrack.Title.Replace("&", "&amp;") + "]]></text></binding></visual></tile>",
                    new XmlLoadSettings { ValidateOnParse = true });
                TileHelper.TileUpdater.Update(new TileNotification(firstXml));
            }
            catch
            {
                // ignored
            }
        }

        private void UpdateNormalTiles()
        {
            if (CurrentTrack == null)
                return;

            if (DeviceHelper.IsXbox)
                return;

            if (!DeviceHelper.IsDesktop && !DeviceHelper.IsMobile) return;

            try
            {
                TileHelper.TileUpdater.Clear();

                var firstXml = new XmlDocument();
                firstXml.LoadXml(
                    "<tile><visual><binding template=\"TileMedium\" branding=\"nameAndLogo\"><image placement=\"peek\" src=\"" +
                    ArtworkConverter.ConvertObjectToImage(CurrentTrack) +
                    "\"/><text>Now Playing</text><text hint-style=\"captionsubtle\" hint-wrap=\"true\"><![CDATA[" +
                    CurrentTrack.Title.Replace("&", "&amp;") +
                    "]]></text></binding><binding template=\"TileLarge\" branding=\"nameAndLogo\"><image placement=\"peek\" src=\"" +
                    ArtworkConverter.ConvertObjectToImage(CurrentTrack) +
                    "\"/><text>Now Playing</text><text hint-style=\"captionsubtle\" hint-wrap=\"true\"><![CDATA[" +
                    CurrentTrack.Title.Replace("&", "&amp;") + "]]></text></binding></visual></tile>",
                    new XmlLoadSettings { ValidateOnParse = true });
                TileHelper.TileUpdater.Update(new TileNotification(firstXml));
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.TrackException(ex, false);
#if  DEBUG
                throw;
#endif
            }
        }

        #endregion    

    }
}