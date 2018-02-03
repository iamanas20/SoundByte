using System;
using System.Linq;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using YoutubeExplode.Models.MediaStreams;
using System.Threading.Tasks;

namespace SoundByte.UWP.ViewModels
{
    public class NowPlayingViewModel : BaseViewModel
    {

        #region Models

        public SoundByteCollection<CommentSource, BaseComment> CommentItems { get; } =
            new SoundByteCollection<CommentSource, BaseComment>();

        #endregion

        #region Event Handlers

        public async void ShowCompactView()
        {
            await App.SwitchToCompactView();
        }


        /// <summary>
        ///     Called when the current playing item changes
        /// </summary>
        private async void Instance_OnCurrentTrackChanged(BaseTrack newTrack)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                var overlay = App.CurrentFrame.FindName("VideoOverlay") as MediaElement;

                if (overlay != null)
                {
                    if (newTrack.ServiceType == ServiceType.YouTube)
                    {
                        overlay.Source = new Uri(newTrack.VideoStreamUrl);
                        overlay.Play();
                    }
                    else
                    {
                        overlay.Opacity = 0;
                        overlay.Pause();
                        overlay.Source = null;
                    }
                }

                // Reload all the comments
                CommentItems.Source.Track = newTrack;
                CommentItems.RefreshItems();
            });
        }

        #endregion

        #region Dispose Handlers

        public override void Dispose()
        {
            // Only clean if we are in the background
            if (!DeviceHelper.IsBackground)
                return;

            // Dispose of playback view model
            PlaybackViewModel?.Dispose();
            PlaybackViewModel = null;

            CleanModel();
        }
        #endregion

        #region Private Variables

        // The comment text box
        private string _commentText;

        // The pin button text
        private string _pinButtonText = "Pin";

        // The like button text
        private string _likeButtonText = "Like";

        // The repost button text
        private string _repostButtonText = "Repost";

        #endregion

        #region Getters and Setters

        public PlaybackViewModel PlaybackViewModel
        {
            get => _playbackViewModel;
            private set
            {
                if (_playbackViewModel == value)
                    return;

                _playbackViewModel = value;
                UpdateProperty();
            }
        }
        private PlaybackViewModel _playbackViewModel;
        #endregion

        #region Enter and Leave ViewModel Handlers

        /// <summary>
        ///     Setup the view model
        /// </summary>
        public async Task SetupModelAsync()
        {
            PlaybackViewModel = new PlaybackViewModel();

            // Bind the method once we know a playback list exists
            PlaybackService.Instance.OnTrackChange += Instance_OnCurrentTrackChanged;
            CommentItems.Source.Track = PlaybackService.Instance.GetCurrentTrack();

            var overlay = App.CurrentFrame.FindName("VideoOverlay") as MediaElement;

            var currentTrack = PlaybackService.Instance.GetCurrentTrack();

            if (overlay != null)
            {
                if (currentTrack == null)
                    return;

                if (currentTrack.ServiceType == ServiceType.YouTube)
                {
                    var mediaStreams = await PlaybackService.Instance.YouTubeClient.GetVideoMediaStreamInfosAsync(currentTrack.TrackId);

                    if (currentTrack.IsLive)
                    {
                        var source = await AdaptiveMediaSource.CreateFromUriAsync(new Uri(mediaStreams.HlsLiveStreamUrl));
                        if (source.Status == AdaptiveMediaSourceCreationStatus.Success)
                        {
                            overlay.SetMediaStreamSource(source.MediaSource);
                        }
                    }
                    else
                    {
                        var videoStreamUrl = mediaStreams.Video.FirstOrDefault(x => x.VideoQuality == VideoQuality.High1080)?.Url;

                        if (string.IsNullOrEmpty(videoStreamUrl))
                            videoStreamUrl = mediaStreams.Video.OrderBy(s => s.VideoQuality).Last()?.Url;

                        overlay.Source = new Uri(videoStreamUrl);
                    }

                    overlay.Play();
                }
                else
                {
                    overlay.Opacity = 0;
                    overlay.Pause();
                    overlay.Source = null;
                }
            }
        }

        public void MakeFullScreen()
        {
            if (!DeviceHelper.IsDeviceFullScreen)
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            else
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
        }

        /// <summary>
        ///     Clean the view model
        /// </summary>
        public void CleanModel()
        {
            // Unbind the events
            PlaybackService.Instance.OnTrackChange -= Instance_OnCurrentTrackChanged;

            var overlay = App.CurrentFrame?.FindName("VideoOverlay") as MediaElement;

            if (overlay != null)
            {
                overlay.Opacity = 0;
                overlay.Pause();
                overlay.Source = null;
            }
        }

        #endregion

        #region Method Bindings

        /// <summary>
        ///     Display the playlist picker if it exists
        /// </summary>
        public async void DisplayPlaylist()
        {
            await NavigationService.Current.CallDialogAsync<PlaylistDialog>(PlaybackService.Instance.GetCurrentTrack());
        }

        /// <summary>
        ///     Navigate to the selected track in the playlist
        /// </summary>
        public void GotoRelatedTrack(object sender, ItemClickEventArgs e)
        {
            PlaybackService.Instance.MoveTo(e.ClickedItem as BaseTrack);
        }

        /// <summary>
        ///     Called when the user taps on a comment.
        ///     This method changes the current position of the track
        ///     to the comment time.
        /// </summary>
        public void GoToCommentItemPosition(object sender, ItemClickEventArgs e)
        {
            // Get the comment object
            var comment = (BaseComment)e.ClickedItem;

            // Set the current position
            PlaybackService.Instance.SetTrackPosition(comment.CommentTime);
        }

        public async void ShareTrack()
        {
            await NavigationService.Current.CallDialogAsync<ShareDialog>(PlaybackService.Instance.GetCurrentTrack());
        }

        /// <summary>
        ///     Navigates the user to the current track users profile
        /// </summary>
        public async void GoToUserProfile()
        {
            // Show the loading ring as loading the user can take
            // some time.
            await App.SetLoadingAsync(true);

            var currentTrack = PlaybackService.Instance.GetCurrentTrack();

            // We only support viewing soundcloud profiles at this time
            if (currentTrack?.ServiceType != ServiceType.SoundCloud)
            {
                await NavigationService.Current.CallMessageDialogAsync(
                    "SoundByte does not currently supporting user profiles that are not from SoundCloud.",
                    "Not Ready Yet");
                return;
            }

            // Get the user object
            var currentUser = await SoundByteService.Current.GetAsync<SoundCloudUser>(ServiceType.SoundCloud, "/users/" + currentTrack.User.UserId);

            // Hide the loading ring
            await App.SetLoadingAsync(false);

            // Navigate to the user page
            App.NavigateTo(typeof(UserView), currentUser.Response.ToBaseUser());
        }

        #endregion
    }
}