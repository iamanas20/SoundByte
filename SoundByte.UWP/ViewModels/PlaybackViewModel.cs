using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using JetBrains.Annotations;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Extensions;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.ViewModels
{
    public class PlaybackViewModel : BaseViewModel
    {
        private readonly CoreDispatcher _currentUiDispatcher;

        #region Getters and Setters
        /// <summary>
        /// Current playlist of items
        /// </summary>
        public ObservableCollection<BaseTrack> Playlist { get; } = new ObservableCollection<BaseTrack>();

        /// <summary>
        ///     Is reposting enabled (only soundcloud)
        /// </summary>
        public bool IsRepostEnabled
        {
            get => _isRepostEnabled;
            set
            {
                if (_isRepostEnabled == value)
                    return;

                _isRepostEnabled = value;
                UpdateProperty();
            }
        }
        private bool _isRepostEnabled;

        /// <summary>
        ///     Has the tile been pined to the start menu
        /// </summary>
        public bool IsTilePined
        {
            get => _isTilePinned;
            set
            {
                if (_isTilePinned == value)
                    return;

                _isTilePinned = value;
                UpdateProperty();
            }
        }
        private bool _isTilePinned;

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
        ///     The current value of the volume slider
        /// </summary>
        public double MediaVolume
        {
            get => PlaybackService.Instance.TrackVolume * 100;
            set
            {
                // Set the volume
                PlaybackService.Instance.SetTrackVolume(value / 100);

                // Update the UI
                if ((int)value == 0)
                {
                    PlaybackService.Instance.MuteTrack(true);
                    VolumeIcon = "\uE74F";
                }
                else if (value < 25)
                {
                    PlaybackService.Instance.MuteTrack(false);
                    VolumeIcon = "\uE992";
                }
                else if (value < 50)
                {
                    PlaybackService.Instance.MuteTrack(false);
                    VolumeIcon = "\uE993";
                }
                else if (value < 75)
                {
                    PlaybackService.Instance.MuteTrack(false);
                    VolumeIcon = "\uE994";
                }
                else
                {
                    PlaybackService.Instance.MuteTrack(false);
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
            get => PlaybackService.Instance.IsPlaylistShuffled;
            set
            {
                // Set the new value and force the UI to update
                PlaybackService.Instance.ShufflePlaylist(value);
                UpdateProperty();
            }
        }

        /// <summary>
        ///     Is the song going to repeat when finished
        /// </summary>
        public bool IsRepeatEnabled
        {
            get => PlaybackService.Instance.IsTrackRepeating;
            set
            {
                PlaybackService.Instance.RepeatTrack(value);
                UpdateProperty();
            }
        }

        #endregion

        #region Timers
        /// <summary>
        ///     This timer runs every 500ms to ensure that the current position,
        ///     time, remaining time, etc. variables are correct.
        /// </summary>
        private readonly DispatcherTimer _updateInformationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };

        /// <summary>
        ///     This timer has to run quite fast. It ensures that audio and video are
        ///     in sync for YouTube videos.
        /// </summary>
        private readonly DispatcherTimer _audioVideoSyncTimer = new DispatcherTimer
        {

            Interval = TimeSpan.FromMilliseconds(75)
        };
        #endregion

        #region Constructors
        public PlaybackViewModel() : this(CoreApplication.MainView.Dispatcher)
        {  }

        public PlaybackViewModel(CoreDispatcher uiDispatcher)
        {
            _currentUiDispatcher = uiDispatcher;

            // Bind the methods that we need
            PlaybackService.Instance.OnStateChange += OnStateChange;
            PlaybackService.Instance.OnTrackChange += OnTrackChange;

            // Bind timer methods
            _updateInformationTimer.Tick += UpdateInformation;
            _audioVideoSyncTimer.Tick += SyncAudioVideo;

            // Update info to current track
            OnTrackChange(PlaybackService.Instance.GetCurrentTrack());
            UpdateUpNext();

            Application.Current.LeavingBackground += CurrentOnLeavingBackground;

            // Start the timer if ready
            if (!_updateInformationTimer.IsEnabled)
                _updateInformationTimer.Start();
        }

        private void CurrentOnLeavingBackground(object sender, LeavingBackgroundEventArgs leavingBackgroundEventArgs)
        {
            // Call the track change method
            OnTrackChange(PlaybackService.Instance.GetCurrentTrack());
        }

        #endregion

        #region Timer Methods
        /// <summary>
        ///     Syncs YouTube audio and video when needed
        /// </summary>
        private async void SyncAudioVideo(object sender, object e)
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
            if (PlaybackService.Instance.CurrentPlaybackState != MediaPlaybackState.Playing
                || PlaybackService.Instance.GetTrackPosition().Milliseconds <= 0)
                return;

            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!(App.CurrentFrame?.FindName("VideoOverlay") is MediaElement overlay))
                    return;

                var difference = overlay.Position - PlaybackService.Instance.GetTrackPosition();

                if (Math.Abs(difference.TotalMilliseconds) >= 1000)
                {
                    overlay.PlaybackRate = 1;
                    overlay.Position = PlaybackService.Instance.GetTrackPosition();
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
            });
        }

        /// <summary>
        ///     Timer method that is run to make sure the UI is kept up to date
        /// </summary>
        private async void UpdateInformation(object sender, object e)
        {
            if (DeviceHelper.IsBackground)
                return;

            // Only call the following if the player exists, is playing
            // and the time is greater then 0.
            if (PlaybackService.Instance.CurrentPlaybackState != MediaPlaybackState.Playing
                || PlaybackService.Instance.GetTrackPosition().Milliseconds <= 0)
                return;

            if (CurrentTrack == null)
                return;

            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
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
                    CurrentTimeValue = PlaybackService.Instance.GetTrackPosition().TotalSeconds;

                    // Get the remaining time for the track
                    var remainingTime = PlaybackService.Instance.GetTrackDuration().Subtract(PlaybackService.Instance.GetTrackPosition());

                    // Set the time listened text
                    TimeListened = NumberFormatHelper.FormatTimeString(PlaybackService.Instance.GetTrackPosition().TotalMilliseconds);

                    // Set the time remaining text
                    TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(remainingTime.TotalMilliseconds);

                    // Set the maximum value
                    MaxTimeValue = PlaybackService.Instance.GetTrackDuration().TotalSeconds;
                }
            });
        }
        #endregion

        #region Track Control Methods
        /// <summary>
        ///     Toggle if the current track should repeat
        /// </summary>
        public void ToggleRepeat()
        {
            IsRepeatEnabled = !IsRepeatEnabled;
        }

        /// <summary>
        ///     Toggle if the current playlist is shuffled
        /// </summary>
        public void ToggleShuffle()
        {
            IsShuffleEnabled = !IsShuffleEnabled;

            UpdateUpNext();
        }

        /// <summary>
        ///     Toggle if we should mute
        /// </summary>
        public void ToggleMute()
        {
            // Toggle mute
            PlaybackService.Instance.MuteTrack(!PlaybackService.Instance.IsTrackMuted);

            // Update the UI
            VolumeIcon = PlaybackService.Instance.IsTrackMuted ? "\uE74F" : "\uE767";
        }

        public void NavigateNowPlaying()
        {
            App.NavigateTo(typeof(NowPlayingView));
        }

        public async void ShareTrack()
        {
            await NavigationService.Current.CallDialogAsync<ShareDialog>(PlaybackService.Instance.GetCurrentTrack());
        }

        public void ViewArtist()
        {
            App.NavigateTo(typeof(UserView), PlaybackService.Instance.GetCurrentTrack()?.User);
        }


        #endregion

        #region Track Playback State
        /// <summary>
        ///     Toggles the state between the track playing
        ///     and not playing
        /// </summary>
        public void ChangePlaybackState()
        {
            // Get the current state of the track
            var currentState = PlaybackService.Instance.CurrentPlaybackState;

            // If the track is currently paused
            if (currentState == MediaPlaybackState.Paused)
            {
                //               UpdateNormalTiles();
                // Play the track
                PlaybackService.Instance.PlayTrack();
            }

            // If the track is currently playing
            if (currentState == MediaPlaybackState.Playing)
            {
                //              UpdatePausedTile();
                // Pause the track
                PlaybackService.Instance.PauseTrack();
            }
        }

        /// <summary>
        ///     Go forward one track
        /// </summary>
        public void SkipNext()
        {
            PlaybackService.Instance.NextTrack();
        }

        /// <summary>
        ///     Go backwards one track
        /// </summary>
        public void SkipPrevious()
        {
            PlaybackService.Instance.PreviousTrack();
        }

        #endregion

        private void UpdateUpNext()
        {
            // Get and convert tracks
            var playlist = (PlaybackService.Instance.MediaPlaybackList.ShuffleEnabled) 
                ? PlaybackService.Instance.MediaPlaybackList.ShuffledItems.Select(x => x.Source.AsBaseTrack())
                : PlaybackService.Instance.MediaPlaybackList.Items.Select(x => x.Source.AsBaseTrack());

            // Clear playlist and add items
            Playlist.Clear();
            foreach (var baseTrack in playlist)
            {
                Playlist.Add(baseTrack);
            }
        }



        #region Methods
        public async void OnPlayingSliderChange()
        {
            if (DeviceHelper.IsBackground)
                return;

            if (CurrentTrack == null)
                return;

            if (CurrentTrack.IsLive)
                return;

            // Set the track position
            PlaybackService.Instance.SetTrackPosition(TimeSpan.FromSeconds(CurrentTimeValue));

            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!(App.CurrentFrame.FindName("VideoOverlay") is MediaElement overlay))
                    return;

                overlay.Position = TimeSpan.FromSeconds(CurrentTimeValue);
            });
        }

        /// <summary>
        ///     Called when the playback service loads a new track. Used
        ///     to update the required values for the UI.
        /// </summary>
        /// <param name="track"></param>
        private async void OnTrackChange(BaseTrack track)
        {
            // Do nothing if running in the background
            if (DeviceHelper.IsBackground)
                return;

            // Same track, no need to perform this logic
            if (track == CurrentTrack)
                return;

            // Run all this on the UI thread
            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
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

                if (!track.IsLive)
                {
                    TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(track.Duration.TotalMilliseconds);
                    TimeListened = "00:00";
                    CurrentTimeValue = 0;
                    MaxTimeValue = track.Duration.TotalSeconds;
                }
                else
                {
                    TimeRemaining = "LIVE";
                    TimeListened = "LIVE";
                    CurrentTimeValue = 1;
                    MaxTimeValue = 1;
                }

                UpdateUpNext();

                // If the current track is a soundcloud track, enabled reposting.
                IsRepostEnabled = track.ServiceType == ServiceType.SoundCloud;

                // Update the tile value
                IsTilePined = TileHelper.IsTilePinned("Track_" + track.TrackId);

                if (CurrentTrack?.ServiceType == ServiceType.SoundCloud)
                {


                    try
                    {
                        CurrentTrack.IsLiked = (await SoundByteService.Current.ExistsAsync(ServiceType.SoundCloud,
                            "/me/favorites/" + CurrentTrack.TrackId)).Response;
                    }
                    catch
                    {
                        CurrentTrack.IsLiked = false;
                    }

                    try
                    {
                        CurrentTrack.User = (await SoundByteService.Current.GetAsync<SoundCloudUser>(ServiceType.SoundCloud, $"/users/{CurrentTrack.User.UserId}")).Response.ToBaseUser();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });
        }

        private async void OnStateChange(MediaPlaybackState mediaPlaybackState)
        {
            // Don't run in the background if on Xbox
            if (DeviceHelper.IsBackground && DeviceHelper.IsXbox)
                return;

            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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
        #endregion

        public override void Dispose()
        {
            // Unbind the methods that we need
            PlaybackService.Instance.OnStateChange -= OnStateChange;
            PlaybackService.Instance.OnTrackChange -= OnTrackChange;

            // Unbind timer methods
            _updateInformationTimer.Tick -= UpdateInformation;
            _audioVideoSyncTimer.Tick -= SyncAudioVideo;

            Application.Current.LeavingBackground -= CurrentOnLeavingBackground;

            base.Dispose();
        }














        #region Method Bindings
        /// <summary>
        ///     Toggle if the track has been liked or not
        /// </summary>
        public async void ToggleLikeTrack()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Like Error");
                return;
            }

            // User is not logged in
            if (!SoundByteService.Current.IsServiceConnected(CurrentTrack.ServiceType))
            {
                await NavigationService.Current.CallMessageDialogAsync($"You must connect your {CurrentTrack.ServiceType} account to do this.", "Like Error");
                return;
            }

            // Toggle like status
            CurrentTrack.ToggleLike();
        }

        /// <summary>
        ///     Toggle is a track has been reposted or not
        /// </summary>
        public async void ToggleRepostTrack()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Repost Error");
                return;
            }

            // Must be SoundCloud track
            if (CurrentTrack.ServiceType != ServiceType.SoundCloud &&
                CurrentTrack.ServiceType != ServiceType.SoundCloudV2)
            {
                await NavigationService.Current.CallMessageDialogAsync("Reposting is only supported on SoundCloud tracks.", "Repost Error");
                return;
            }

            // User is not logged in
            if (!SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud))
            {
                await NavigationService.Current.CallMessageDialogAsync("You must connect your SoundCloud account to do this.", "Repost Error");
                return;
            }

            try
            {
                if (!(await SoundByteService.Current.PutAsync(ServiceType.SoundCloud, $"/e1/me/track_reposts/{CurrentTrack.TrackId}")).Response)
                {
                    await NavigationService.Current.CallMessageDialogAsync("Unknown Error", "Repost Error");
                }
            }
            catch (Exception e)
            {
                await NavigationService.Current.CallMessageDialogAsync(e.Message, "Repost Error");
            }
        }

        public async void TogglePinTile()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Pin Tile Error");
                return;
            }

            // Check if the tile exists
            var tileExists = TileHelper.IsTilePinned("Track_" + CurrentTrack.TrackId);

            if (tileExists)
            {
                // Remove the tile and check if it was successful
                if (await TileHelper.RemoveTileAsync("Track_" + CurrentTrack.TrackId))
                {
                    IsTilePined = false;
                }
                else
                {
                    IsTilePined = true;
                }
            }
            else
            {
                // Create a live tile and check if it was created
                if (await TileHelper.CreateTileAsync("Track_" + CurrentTrack.TrackId,
                    CurrentTrack.Title, "soundbyte://core/track?id=" + CurrentTrack.TrackId,
                    new Uri(CurrentTrack.ThumbnailUrl), ForegroundText.Light))
                {
                    IsTilePined = true;
                }
                else
                {
                    IsTilePined = false;
                }
            }
        }



        public async void DisplayPlaylist()
        {
            await NavigationService.Current.CallDialogAsync<PlaylistDialog>(PlaybackService.Instance.GetCurrentTrack());
        }




        #endregion
    }
}
