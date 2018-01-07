/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Media.Playback;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Helpers;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     The centeral way of accessing playback within the
    ///     app, provides access the the media player and active
    ///     playlist.
    /// </summary>
    [Obsolete]
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
        #endregion

        #region Class Variables / Getters and Setters
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
        [Obsolete]
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
            get => PlaybackV2Service.Instance.TrackVolume * 100;
            set
            {
                // Set the volume
                PlaybackV2Service.Instance.SetTrackVolume(value / 100);

                // Update the UI
                if ((int)value == 0)
                {
                    PlaybackV2Service.Instance.MuteTrack(true);
                    VolumeIcon = "\uE74F";
                }
                else if (value < 25)
                {
                    PlaybackV2Service.Instance.MuteTrack(false);
                    VolumeIcon = "\uE992";
                }
                else if (value < 50)
                {
                    PlaybackV2Service.Instance.MuteTrack(false);
                    VolumeIcon = "\uE993";
                }
                else if (value < 75)
                {
                    PlaybackV2Service.Instance.MuteTrack(false);
                    VolumeIcon = "\uE994";
                }
                else
                {
                    PlaybackV2Service.Instance.MuteTrack(false);
                    VolumeIcon = "\uE767";
                }

                UpdateProperty();
            }
        }

        /// <summary>
        ///     Are tracks shuffled
        /// </summary>
        public bool IsShuffleEnabled
        {
            get => PlaybackV2Service.Instance.IsPlaylistShuffled;
            set
            {
                // Set the new value and force the UI to update
                PlaybackV2Service.Instance.ShufflePlaylist(value);
                UpdateProperty();
            }
        }

        /// <summary>
        ///     Is the song going to repeat when finished
        /// </summary>
        public bool IsRepeatEnabled
        {
            get => PlaybackV2Service.Instance.IsTrackRepeating;
            set
            {
                PlaybackV2Service.Instance.RepeatTrack(value);
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
            PlaybackV2Service.Instance.OnStateChange += PlaybackSessionStateChanged;
            PlaybackV2Service.Instance.OnTrackChange += CurrentItemChanged;

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

            App.Telemetry.TrackEvent("Toggle Repeat");
        }

        /// <summary>
        ///     Toggle if we should shuffle the items
        /// </summary>
        public void ToggleShuffle()
        {
            IsShuffleEnabled = !IsShuffleEnabled;

            App.Telemetry.TrackEvent("Toggle Shuffle");
        }

        /// <summary>
        ///     Toggle the media mute
        /// </summary>
        public void ToggleMute()
        {
            // Toggle the mute
            PlaybackV2Service.Instance.MuteTrack(!PlaybackV2Service.Instance.IsTrackMuted);

            // Update the UI
            VolumeIcon = PlaybackV2Service.Instance.IsTrackMuted ? "\uE74F" : "\uE767";
        }

        /// <summary>
        ///     Toggles the state between the track playing
        ///     and not playing
        /// </summary>
        public void ChangePlaybackState()
        {
            // Get the current state of the track
            var currentState = PlaybackV2Service.Instance.CurrentPlaybackState;

            // If the track is currently paused
            if (currentState == MediaPlaybackState.Paused)
            {
                UpdateNormalTiles();
                // Play the track
                PlaybackV2Service.Instance.PlayTrack();
            }

            // If the track is currently playing
            if (currentState == MediaPlaybackState.Playing)
            {
                UpdatePausedTile();
                // Pause the track
                PlaybackV2Service.Instance.PauseTrack();
            }
        }

        /// <summary>
        ///     Go forward one track
        /// </summary>
        public void SkipNext()
        {
            PlaybackV2Service.Instance.NextTrack();
        }

        /// <summary>
        ///     Go backwards one track
        /// </summary>
        public void SkipPrevious()
        {
            PlaybackV2Service.Instance.PreviousTrack();
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
            if (PlaybackV2Service.Instance.CurrentPlaybackState != MediaPlaybackState.Playing
                || PlaybackV2Service.Instance.GetTrackPosition().Milliseconds <= 0)
                return;

            var overlay = App.CurrentFrame?.FindName("VideoOverlay") as MediaElement;
            if (overlay == null) return;

            var difference = overlay.Position - PlaybackV2Service.Instance.GetTrackPosition();

            if (Math.Abs(difference.TotalMilliseconds) >= 1000)
            {
                overlay.PlaybackRate = 1;
                overlay.Position = PlaybackV2Service.Instance.GetTrackPosition();
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
            if (PlaybackV2Service.Instance.CurrentPlaybackState != MediaPlaybackState.Playing
                || PlaybackV2Service.Instance.GetTrackPosition().Milliseconds <= 0)
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
                CurrentTimeValue = PlaybackV2Service.Instance.GetTrackPosition().TotalSeconds;

                // Get the remaining time for the track
                var remainingTime = PlaybackV2Service.Instance.GetTrackDuration().Subtract(PlaybackV2Service.Instance.GetTrackPosition());

                // Set the time listened text
                TimeListened = NumberFormatHelper.FormatTimeString(PlaybackV2Service.Instance.GetTrackPosition().TotalMilliseconds);

                // Set the time remaining text
                TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(remainingTime.TotalMilliseconds);

                // Set the maximum value
                MaxTimeValue = PlaybackV2Service.Instance.GetTrackDuration().TotalSeconds;
            }
        }
        #endregion

   
        public class PlaybackResponse
        {
            public PlaybackResponse(bool success, string messsage)
            {
                Success = success;
                Message = messsage;
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
        [Obsolete("This method restricts future development")]
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
            // WRAP V2 SERVICE

            await App.SetLoadingAsync(true);

            var result = await PlaybackV2Service.Instance.InitilizePlaylistAsync<TSource>(trackSource, trackSource.Token);

            if (!result.Success)
                return new PlaybackResponse(false, result.Message);

            await PlaybackV2Service.Instance.StartTrackAsync(startingItem);

            await App.SetLoadingAsync(false);

            return new PlaybackResponse(true, string.Empty);
        }
        #endregion

        #region General Event Handlers
        /// <summary>
        ///     Called when the playback session changes
        /// </summary>
        private async void PlaybackSessionStateChanged(MediaPlaybackState mediaPlaybackState)
        {
            // Don't run in the background if on Xbox
            if (DeviceHelper.IsBackground && DeviceHelper.IsXbox)
                return;

            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                var overlay = App.CurrentFrame?.FindName("VideoOverlay") as MediaElement;

                switch (mediaPlaybackState)
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

        private async void CurrentItemChanged(BaseTrack track)
        {
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
                        CurrentTrack.IsLiked = await SoundByteService.Current.ExistsAsync(ServiceType.SoundCloud,
                            "/me/favorites/" + CurrentTrack.Id);
                    }
                    catch
                    {
                        CurrentTrack.IsLiked = false;
                    }

                    try
                    {
                        CurrentTrack.User = (await SoundByteService.Current.GetAsync<SoundCloudUser>(ServiceType.SoundCloud, $"/users/{CurrentTrack.User.Id}")).ToBaseUser();
                    }
                    catch
                    {
                        // ignored
                    }
                }
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
            PlaybackV2Service.Instance.SetTrackPosition(TimeSpan.FromSeconds(CurrentTimeValue));

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var overlay = App.CurrentFrame.FindName("VideoOverlay") as MediaElement;
                if (overlay == null) return;

                overlay.Position = TimeSpan.FromSeconds(CurrentTimeValue);
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
                App.Telemetry.TrackException(ex, false);
#if  DEBUG
                throw;
#endif
            }
        }

        #endregion    
    }
}