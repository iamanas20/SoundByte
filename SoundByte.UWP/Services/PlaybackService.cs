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
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp;
using SoundByte.API.Endpoints;
using SoundByte.Core.Converters;
using SoundByte.Core.Helpers;
using SoundByte.Core.Services;
using YoutubeExplode;
using User = SoundByte.API.Endpoints.User;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     The centeral way of accessing playback within the
    ///     app, provides access the the media player and active
    ///     playlist.
    /// </summary>
    public class PlaybackService : INotifyPropertyChanged
    {
        private PlaybackService()
        {
            // Create the player instance
            Player = new MediaPlayer { AutoPlay = false, Volume = SettingsService.Instance.PlaybackVolume };

            var adjustedVolume = SettingsService.Instance.PlaybackVolume * 100;

            if ((int)adjustedVolume == 0)
            {
                Player.IsMuted = true;
                VolumeIcon = "\uE74F";
            }
            else if (adjustedVolume < 25)
            {
                Player.IsMuted = false;
                VolumeIcon = "\uE992";
            }
            else if (adjustedVolume < 50)
            {
                Player.IsMuted = false;
                VolumeIcon = "\uE993";
            }
            else if (adjustedVolume < 75)
            {
                Player.IsMuted = false;
                VolumeIcon = "\uE994";
            }
            else
            {
                Player.IsMuted = false;
                VolumeIcon = "\uE767";
            }

            Player.PlaybackSession.PlaybackStateChanged += PlaybackSessionStateChanged;

            var pageTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };

            _tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication("App");
            _tileUpdater.EnableNotificationQueue(true);

            // Setup the tick event
            pageTimer.Tick += PlayingSliderUpdate;

            // If the timer is ready, start it
            if (!pageTimer.IsEnabled)
                pageTimer.Start();
        }

        public Track CurrentTrack
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

        /// <summary>
        ///     The current text for the volume icon
        /// </summary>
        public string VolumeIcon
        {
            get => _volumeIcon;
            set
            {
                if (_volumeIcon == value)
                    return;

                _volumeIcon = value;
                UpdateProperty();
            }
        }

        /// <summary>
        ///     The current value of the volume slider
        /// </summary>
        public double MediaVolume
        {
            get => SettingsService.Instance.PlaybackVolume * 100;
            set
            {
                UpdateProperty();

                // Set the volume
                Player.Volume = value / 100;
                SettingsService.Instance.PlaybackVolume = value / 100;

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

        /// <summary>
        ///     The max slider value
        /// </summary>
        public double MaxTimeValue
        {
            get => _maxTimeValue;
            set
            {
                _maxTimeValue = value;
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
        ///     The data model of the active playlist.
        /// </summary>
        public ObservableCollection<Track> Playlist { get; set; } = new ObservableCollection<Track>();

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
                if(Player == null) return;

                if (Player.IsLoopingEnabled == value)
                    return;

                Player.IsLoopingEnabled = value;
                UpdateProperty();
            }
        }

        /// <summary>
        ///     Called when the playback session changes
        /// </summary>
        private async void PlaybackSessionStateChanged(MediaPlaybackSession sender, object args)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.Playing:
                        App.IsLoading = false;
                        PlayButtonContent = "\uE769";
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
                        break;
                    default:
                        App.IsLoading = false;
                        PlayButtonContent = "\uE768";
                        break;
                }
            });
        }

        public async void LikeTrack()
        {
            if (CurrentTrack == null)
                return;

            // Check to see what the existing string is
            if (LikeIcon == "\uEB52")
            {
                // Delete the like from the users likes and see if successful
                if (await SoundByteService.Instance.DeleteAsync("/e1/me/track_likes/" + CurrentTrack.Id))
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
                if (await SoundByteService.Instance.PutAsync("/e1/me/track_likes/" + CurrentTrack.Id))
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

        private async void CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            // If there is no new item, don't do anything
            if (args.NewItem == null)
                return;

            // Try get the track
            var track = Playlist.FirstOrDefault(
                x => x.Id == (string) args.NewItem.Source.CustomProperties["SoundByteItem.ID"]);

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

                TimeRemaining = "-00:00";
                TimeListened = "00:00";
                CurrentTimeValue = 0;
                MaxTimeValue = 0;

                // Update the live tile
                UpdateNormalTiles();

                try
                {
                    LikeIcon = await SoundByteService.Instance.ExistsAsync("/me/favorites/" + CurrentTrack.Id)
                        ? "\uEB52"
                        : "\uEB51";
                }
                catch
                {
                    LikeIcon = "\uEB51";
                }

                try
                {
                    CurrentTrack.User = await SoundByteService.Instance.GetAsync<User>($"/users/{CurrentTrack.User.Id}");
                }
                catch
                {
                    // ignored
                }
            });

            TelemetryService.Instance.TrackEvent("Background Song Change", new Dictionary<string, string>
            {
                {"PlaylistCount", Playlist.Count.ToString()},
                {"CurrentUsage", MemoryManager.AppMemoryUsage / 1024 / 1024 + "M"},
                {"SoundCloudConnected", SoundByteService.Instance.IsSoundCloudAccountConnected.ToString()},
                {"FanburstConnected", SoundByteService.Instance.IsFanBurstAccountConnected.ToString()},
            });
        }

        private static async Task<string> GetCorrectApiKey()
        {
            return await Task.Run(async () =>
            {
                // Prefer getting the key from the Grid Entertainment Website. More efficient
                var geKey = await SoundByteService.Instance.GridEntertainmentSoundByteGetPlaybackKey();

                if (!string.IsNullOrEmpty(geKey))
                {
                    if (await SoundByteService.Instance.ApiCheck("https://api.soundcloud.com/tracks/320126814/stream?client_id=" + geKey))
                        return geKey;
                }

                // Log this event so I know if the web service fails.
                TelemetryService.Instance.TrackEvent("Old Playback Key Method");

                // Check if we have hit the soundcloud api limit
                if (await SoundByteService.Instance.ApiCheck(
                    "https://api.soundcloud.com/tracks/320126814/stream?client_id=" + ApiKeyService.SoundCloudClientId))
                    return ApiKeyService.SoundCloudClientId;

                // Loop through all the backup keys
                foreach (var key in ApiKeyService.SoundCloudPlaybackClientIds)
                    if (await SoundByteService.Instance.ApiCheck(
                        "https://api.soundcloud.com/tracks/320126814/stream?client_id=" + key))
                        return key;

                return ApiKeyService.SoundCloudClientId;
            });
        }

        /// <summary>
        ///     Playlist a list of tracks with optional values.
        /// </summary>
        /// <param name="playlist">The playlist (list of tracks) that we want to play.</param>
        /// <param name="token">Unique token for this list. Is used to load playback items from cache.</param>
        /// <param name="isShuffled">Should the tracks be played shuffled.</param>
        /// <param name="startingItem">What track to start with.</param>
        /// <returns></returns>
        public async Task<(bool success, string message)> StartMediaPlayback(List<Track> playlist, string token,
            bool isShuffled = false, Track startingItem = null)
        {
            // If no playlist was specified, skip
            if (playlist == null || playlist.Count == 0)
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
                if (token != TokenValue)
                {
                    // Clear the playback list
                    _playbackList.Items.Clear();

                    // Clear the internal list
                    Playlist.Clear();
                }
            }

            // Set the shuffle
            _playbackList.ShuffleEnabled = isShuffled;

            // Get the API key that we will need
            var apiKey = await GetCorrectApiKey();

            // Loop through all the tracks
            foreach (var track in playlist)
            {
                try
                {
                    // If the track is null, leave it alone
                    if (track == null)
                        continue;

                    if (track.ServiceType == ServiceType.YouTube && string.IsNullOrEmpty(track.StreamUrl))
                        continue;

                    MediaSource source;

                    switch (track.ServiceType)
                    {
                        case ServiceType.SoundCloud:
                            source = MediaSource.CreateFromUri(new Uri("http://api.soundcloud.com/tracks/" + track.Id + "/stream?client_id=" + apiKey));
                            break;
                        case ServiceType.Fanburst:
                            source = MediaSource.CreateFromUri(new Uri("https://api.fanburst.com/tracks/" + track.Id + "/stream?client_id=" + ApiKeyService.FanburstClientId));
                            break;
                        case ServiceType.YouTube:
                            source = MediaSource.CreateFromUri(new Uri(track.StreamUrl));
                            break;
                        default:
                            source = MediaSource.CreateFromUri(new Uri("http://api.soundcloud.com/tracks/" + track.Id + "/stream?client_id=" + apiKey));
                            break;
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
                    Playlist.Add(track);
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

            // Update the stored token
            TokenValue = token;

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

        /// <summary>
        ///     Called when the user adjusts the playing slider
        /// </summary>
        public async void PlayingSliderChange()
        {
            // Set the track position
            Instance.Player.PlaybackSession.Position = TimeSpan.FromSeconds(CurrentTimeValue);

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var overlay = App.CurrentFrame.FindName("VideoOverlay") as MediaElement;

                if (overlay == null) return;

                if (CurrentTrack.ServiceType == ServiceType.YouTube)
                {
                    overlay.Position = PlaybackService.Instance.Player.PlaybackSession.Position;
                }
            });
        }

        ~PlaybackService()
        {
            _tileUpdater.Clear();
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


        //// --------------------------------- NEXT GEN v2.1 ITEMS --------------------------------- ////


        public void ToggleRepeat()
        {
            IsRepeatEnabled = !IsRepeatEnabled;

            TelemetryService.Instance.TrackEvent("Toggle Repeat");
        }

        public void ToggleShuffle()
        {
            IsShuffleEnabled = !IsShuffleEnabled;

            TelemetryService.Instance.TrackEvent("Toggle Shuffle");
        }

        #region Private Variables

        private readonly TileUpdater _tileUpdater;

        // Playlist Object
        private MediaPlaybackList _playbackList;

        // The current playing track
        private Track _currentTrack;

        // The amount of time spent listening to the track
        private string _timeListened = "00:00";

        // The amount of time remaining
        private string _timeRemaining = "-00:00";

        // The current slider value
        private double _currentTimeValue;

        // The max slider value
        private double _maxTimeValue = 100;

        // The volume icon text
        private string _volumeIcon = "\uE767";

        private string _likeIcon = "\uEB51";


        public string TokenValue { get; set; }

        // The content on the play_pause button
        private string _playButtonContent = "\uE769";

        #endregion

        #region Live Tiles

        private void UpdatePausedTile()
        {
            if (CurrentTrack == null)
                return;

            if (!DeviceHelper.IsDesktop && !DeviceHelper.IsMobile) return;

            try
            {
                _tileUpdater.Clear();

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
                _tileUpdater.Update(new TileNotification(firstXml));
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
                _tileUpdater.Clear();

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
                _tileUpdater.Update(new TileNotification(firstXml));
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        #region Service Setup

        private static readonly Lazy<PlaybackService> InstanceHolder =
            new Lazy<PlaybackService>(() => new PlaybackService());

        public static PlaybackService Instance => InstanceHolder.Value;

        #endregion

        #region Media Navigation Controls

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
    }
}