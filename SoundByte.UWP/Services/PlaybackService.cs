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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Helpers;
using SoundByte.Core.Services;
using SoundByte.UWP.DatabaseContexts;
using SoundByte.UWP.Models;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     The centeral way of accessing playback within the
    ///     app, provides access the the media player and active
    ///     playlist.
    /// </summary>
    public class PlaybackService : INotifyPropertyChanged
    {
        #region Class Variables / Getters and Setters

        // Playlist Object
        private MediaPlaybackList _playbackList;

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
        /// 
        /// </summary>
        public string LikeIcon
        {
            get => _likeIcon;
            set
            {
                if (_likeIcon == value)
                    return;

                _likeIcon = value;
                UpdateProperty();
            }
        }
        private string _likeIcon = "\uEB51";

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
        public BaseTrackModel Playlist
        {
            get => _playlist;
            private set
            {
                _playlist = value;
                UpdateProperty();
            }
        }
        private BaseTrackModel _playlist = new BaseTrackModel();

        /// <summary>
        ///     The current value of the volume slider
        /// </summary>
        public double MediaVolume
        {
            get => Player.Volume * 100;
            set
            {
                UpdateProperty();

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

            // The page timer is used to update things like current position, time
            // remaining time etc.
            var pageTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            // The audio-video sync timer is used to sync YouTube videos
            // to the background audio. This has to run a little faster for
            // a smoother expirence.
            var audioVideoSyncTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(75)
            };

            // Setup the tick event
            pageTimer.Tick += PlayingSliderUpdate;
            audioVideoSyncTimer.Tick += SyncAudioVideo;

            // If the timer is ready, start it
            if (!pageTimer.IsEnabled)
                pageTimer.Start();

            if (!audioVideoSyncTimer.IsEnabled)
                audioVideoSyncTimer.Start();
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

            var overlay = App.CurrentFrame.FindName("VideoOverlay") as MediaElement;
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
        #endregion

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
        public async Task<(bool success, string message)> StartPlaylistMediaPlaybackAsync(IEnumerable<BaseTrack> playlist,
            bool isShuffled = false, BaseTrack startingItem = null)
        {
            // Create a dummy base track items
            var dummyBaseTrackModel = new BaseTrackModel { Token = "eol" };

            // Add all the playlist items to this dummy
            foreach (var track in playlist)
                dummyBaseTrackModel.Add(track);

            // Call the normal method
            return await StartModelMediaPlaybackAsync(dummyBaseTrackModel, isShuffled, startingItem);
        }

        /// <summary>
        ///     Playlist a list of tracks with optional values.
        /// </summary>
        /// <param name="model">The model for the tracks we want to play</param>
        /// <param name="isShuffled">Should the tracks be played shuffled.</param>
        /// <param name="startingItem">What track to start with.</param>
        /// <returns></returns>
        public async Task<(bool success, string message)> StartModelMediaPlaybackAsync(BaseTrackModel model,
            bool isShuffled = false, BaseTrack startingItem = null)
        {
            // If no playlist was specified, skip
            if (model == null || model.Count == 0)
                return (false,
                    "The playback list was missing or empty. This can be caused if there are not tracks avaliable (for example, you are trying to play your likes, but have not liked anything yet).\n\nAnother reason for this message is that if your playing a track from SoundCloud, SoundCloud has blocked these tracks from being played on 3rd party apps (such as SoundByte).");

            // Pause Everything
            Player.Pause();

            // If the playback list is not null, run this
            if (_playbackList == null)
            {
                // Create the new playback list (with autorepeat enabled)
                _playbackList = new MediaPlaybackList
                {
                    AutoRepeatEnabled = true
                };

                // Subscribe to the item change event
                _playbackList.CurrentItemChanged += CurrentItemChanged;
            }
            else
            {
                // If the tokens do not match, reload the list
                //   if (token != TokenValue)
                //   {
                // Clear the playback list
                _playbackList.Items.Clear();

                // Clear the internal list
                //   }
            }

            // Set the model
            Playlist = model;

            // Set the shuffle
            _playbackList.ShuffleEnabled = isShuffled;

            // The soundcloud API key
            var soundCloudApiKey = string.Empty;

            // If the playlist contains any soundcloud tracks, we need to 
            // grab the appropiate client ID.
            if (model.Any(x => x.ServiceType == ServiceType.SoundCloud))
                soundCloudApiKey = await GetCorrectApiKey();

            // Loop through all the tracks
            foreach (var track in model)
            {
                try
                {
                    // If the track is null, leave it alone
                    if (track == null)
                        continue;

                    MediaSource source;

                    switch (track.ServiceType)
                    {
                        case ServiceType.SoundCloud:
                            source = MediaSource.CreateFromUri(new Uri("http://api.soundcloud.com/tracks/" + track.Id + "/stream?client_id=" + soundCloudApiKey));
                            break;
                        case ServiceType.Fanburst:
                            source = MediaSource.CreateFromUri(new Uri("https://api.fanburst.com/tracks/" + track.Id + "/stream?client_id=" + ApiKeyService.FanburstClientId));
                            break;
                        case ServiceType.YouTube:
                            source = MediaSource.CreateFromUri(new Uri(track.AudioStreamUrl));
                            break;
                        default:
                            throw new Exception("Unknown Track Type: " + track.ServiceType);
                    }

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
                    _playbackList.Items.Add(playbackItem);
                }
                catch (Exception)
                {
                    TelemetryService.Instance.TrackEvent("Could not add Playback Item",
                        new Dictionary<string, string>
                        {
                            {"track_id", track.Id}
                        });
                }
            }
            // Update the controls that we are changing track
            Player.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;

            // Set the playback list
            Player.Source = _playbackList;

            // If the track is shuffled, or no starting item is supplied, just play as usual
            if (isShuffled || startingItem == null)
            {
                Player.Play();
                return (true, string.Empty);
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

                    return (true, string.Empty);
                }
                catch (Exception)
                {
                    keepTrying++;
                    await Task.Delay(200);
                }

            if (keepTrying < 50) return (true, string.Empty);

            TelemetryService.Instance.TrackEvent("Playback Could not Start", new Dictionary<string, string>
            {
                {"track_id", startingItem.Id}
            });

            return (false, "SoundByte could not play this track or list of tracks. Try again later.");
        }
        #endregion

        #region General Event Handlers
        /// <summary>
        ///     Called when the playback session changes
        /// </summary>
        private async void PlaybackSessionStateChanged(MediaPlaybackSession sender, object args)
        {
            // Don't run in the background
            if (DeviceHelper.IsBackground)
                return;

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var overlay = App.CurrentFrame.FindName("VideoOverlay") as MediaElement;

                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.Playing:
                        App.IsLoading = false;
                        PlayButtonContent = "\uE769";
                        overlay?.Play();
                        break;
                    case MediaPlaybackState.Buffering:
                        App.IsLoading = true;
                        break;
                    case MediaPlaybackState.None:
                        App.IsLoading = false;
                        PlayButtonContent = "\uE768";
                        break;
                    case MediaPlaybackState.Opening:
                        App.IsLoading = true;
                        break;
                    case MediaPlaybackState.Paused:
                        App.IsLoading = false;
                        PlayButtonContent = "\uE768";
                        overlay?.Pause();
                        break;
                    default:
                        App.IsLoading = false;
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

            // Run all this on the UI thread
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                if (DeviceHelper.IsBackground && DeviceHelper.IsXbox)
                    return;

                // Set the new current track, updating the UI
                CurrentTrack = track;

                TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(CurrentTrack.Duration.TotalMilliseconds);
                TimeListened = "00:00";
                CurrentTimeValue = 0;
                MaxTimeValue = CurrentTrack.Duration.TotalSeconds;

                // Set the last playback date
                CurrentTrack.LastPlaybackDate = DateTime.UtcNow;

                // Update the live tile
                UpdateNormalTiles();

                if (CurrentTrack?.ServiceType == ServiceType.SoundCloud)
                {
                    try
                    {
                        LikeIcon = await SoundByteV3Service.Current.ExistsAsync(ServiceType.SoundCloud, "/me/favorites/" + CurrentTrack.Id)
                            ? "\uEB52"
                            : "\uEB51";
                    }
                    catch
                    {
                        LikeIcon = "\uEB51";
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

            using (var db = new HistoryContext())
            {
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

            TelemetryService.Instance.TrackEvent("Background Song Change", new Dictionary<string, string>
            {
                {"PlaylistCount", Playlist.Count.ToString()},
                {"CurrentUsage", MemoryManager.AppMemoryUsage / 1024 / 1024 + "M"},
                {"TrackType", CurrentTrack?.ServiceType.ToString() ?? "Null"},
            });
        }

        /// <summary>
        ///     Called when the user adjusts the playing slider
        /// </summary>
        public async void PlayingSliderChange()
        {
            if (DeviceHelper.IsBackground)
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
        private static async Task<string> GetCorrectApiKey()
        {
            return await Task.Run(async () =>
            {
                // Check if we have hit the soundcloud api limit
                if (await ApiCheck(
                    $"https://api.soundcloud.com/tracks/320126814/stream?client_id={ApiKeyService.SoundCloudClientId}"))
                    return ApiKeyService.SoundCloudClientId;

                // Loop through all the backup keys
                foreach (var key in ApiKeyService.SoundCloudPlaybackClientIds)
                    if (await ApiCheck(
                        $"https://api.soundcloud.com/tracks/320126814/stream?client_id={key}"))
                        return key;

                return ApiKeyService.SoundCloudClientId;
            });
        }
        #endregion
       
        #region Live Tiles

        private void UpdatePausedTile()
        {
            if (CurrentTrack == null)
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
                TelemetryService.Instance.TrackException(ex);
#if  DEBUG
                throw;
#endif
            }
        }

        #endregion    

        public async void LikeTrack()
        {
            if (CurrentTrack == null)
                return;

            // Check to see what the existing string is
            if (LikeIcon == "\uEB52")
            {
                // Delete the like from the users likes and see if successful
                if (await SoundByteV3Service.Current.DeleteAsync(ServiceType.SoundCloud, "/e1/me/track_likes/" + CurrentTrack.Id))
                {
                    LikeIcon = "\uEB51";
                    // Track Event
                    TelemetryService.Instance.TrackEvent("Unlike Track");
                }
                else
                {
                    LikeIcon = "\uEB52";
                }
            }
            else
            {
                // Add a like to the users likes and see if successful
                if (await SoundByteV3Service.Current.PutAsync(ServiceType.SoundCloud, $"/e1/me/track_likes/{CurrentTrack.Id}"))
                {
                    LikeIcon = "\uEB52";
                    // Track Event
                    TelemetryService.Instance.TrackEvent("Like Track");
                }
                else
                {
                    LikeIcon = "\uEB51";
                }
            }
        }       
    }
}