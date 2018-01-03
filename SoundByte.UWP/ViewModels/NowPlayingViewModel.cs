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
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;

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
             
                // Set the pin button text
                PinButtonText = TileHelper.IsTilePinned("Track_" + newTrack.Id) ? "Unpin" : "Pin";

                // Set the like button text
                LikeButtonText = await SoundByteService.Current.ExistsAsync(ServiceType.SoundCloud, "/me/favorites/" + newTrack.Id)
                    ? "Unlike"
                    : "Like";

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

        /// <summary>
        ///     The text on the pin button
        /// </summary>
        public string PinButtonText
        {
            get => _pinButtonText;
            set
            {
                if (_pinButtonText == value)
                    return;

                _pinButtonText = value;
                UpdateProperty();
            }
        }

        /// <summary>
        ///     The text on the like button
        /// </summary>
        public string LikeButtonText
        {
            get => _likeButtonText;
            set
            {
                if (_likeButtonText == value)
                    return;

                _likeButtonText = value;
                UpdateProperty();
            }
        }

        /// <summary>
        ///     The text on the repost button
        /// </summary>
        public string RepostButtonText
        {
            get => _repostButtonText;
            set
            {
                if (_repostButtonText == value)
                    return;

                _repostButtonText = value;
                UpdateProperty();
            }
        }

        /// <summary>
        ///     The comment text for the comment box
        /// </summary>
        public string CommentText
        {
            get => _commentText;

            set
            {
                if (_commentText == value)
                    return;

                _commentText = value;
                UpdateProperty();
            }
        }

        #endregion

        #region Enter and Leave ViewModel Handlers

        /// <summary>
        ///     Setup the view model
        /// </summary>
        public void SetupModel()
        {
            // Bind the method once we know a playback list exists
            PlaybackService.Instance.OnCurrentTrackChanged += Instance_OnCurrentTrackChanged;

            CommentItems.Source.Track = Service.CurrentTrack;

            var overlay = App.CurrentFrame.FindName("VideoOverlay") as MediaElement;

            if (overlay != null)
            {
                if (Service.CurrentTrack == null)
                    return;

                if (Service.CurrentTrack.ServiceType == ServiceType.YouTube)
                {
                    overlay.Source = new Uri(Service.CurrentTrack.VideoStreamUrl);
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
            PlaybackService.Instance.OnCurrentTrackChanged -= Instance_OnCurrentTrackChanged;

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
            await NavigationService.Current.CallDialogAsync<PlaylistDialog>(Service.CurrentTrack);
        }

        /// <summary>
        ///     Navigate to the selected track in the playlist
        /// </summary>
        public async void GotoRelatedTrack(object sender, ItemClickEventArgs e)
        {
            var startPlayback =
                await PlaybackService.Instance.StartPlaylistMediaPlaybackAsync(PlaybackService.Instance.Playlist.ToList(), false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing related track.").ShowAsync();
        }

        /// <summary>
        ///     Pin the tile to the start menu
        /// </summary>
        public async void PinTile()
        {
            if (Service.CurrentTrack != null)
            {
                // Check if the tile exists
                var tileExists = TileHelper.IsTilePinned("Track_" + Service.CurrentTrack.Id);

                if (tileExists)
                {
                    // Remove the tile and check if it was successful
                    if (await TileHelper.RemoveTileAsync("Track_" + Service.CurrentTrack.Id))
                    {
                        PinButtonText = "Pin";
                        // Track Event
                        App.Telemetry.TrackEvent("Unpin Track");
                    }
                    else
                    {
                        PinButtonText = "Unpin";
                    }
                }
                else
                {
                    // Create a live tile and check if it was created
                    if (await TileHelper.CreateTileAsync("Track_" + Service.CurrentTrack.Id,
                        Service.CurrentTrack.Title, "soundbyte://core/track?id=" + Service.CurrentTrack.Id,
                        new Uri(ArtworkConverter.ConvertObjectToImage(Service.CurrentTrack)), ForegroundText.Light))
                    {
                        PinButtonText = "Unpin";
                        // Track Event
                        App.Telemetry.TrackEvent("Pin Track");
                    }
                    else
                    {
                        PinButtonText = "Pin";
                    }
                }
            } 
        }

        /// <summary>
        ///     Repost the current track to the users stream
        /// </summary>
        public async void RepostTrack()
        {
            if (Service.CurrentTrack == null)
                return;

            // Check to see what the existing string is
            if (RepostButtonText == "Unpost")
            {
                // Delete the repost value and check if it was successful
                if (await SoundByteService.Current.DeleteAsync(ServiceType.SoundCloud, "/e1/me/track_reposts/" + Service.CurrentTrack.Id))
                {
                    RepostButtonText = "Repost";
                    // Track Event
                    App.Telemetry.TrackEvent("Unpost Track");
                }
                else
                {
                    RepostButtonText = "Unpost";
                }
            }
            else
            {
                // Put a value in the reposted tracks and see if successful
                if (await SoundByteService.Current.PutAsync(ServiceType.SoundCloud, $"/e1/me/track_reposts/{Service.CurrentTrack.Id}"))
                {
                    RepostButtonText = "Unpost";
                    // Track Event
                    App.Telemetry.TrackEvent("Repost Track");
                }
                else
                {
                    RepostButtonText = "Repost";
                }
            }
        }

        /// <summary>
        ///     Like the current track
        /// </summary>
        public async void LikeTrack()
        {
            if (Service.CurrentTrack == null)
                return;

            // Check to see what the existing string is
            if (LikeButtonText == "Unlike")
            {
                // Delete the like from the users likes and see if successful
                if (await SoundByteService.Current.DeleteAsync(ServiceType.SoundCloud, "/e1/me/track_likes/" + Service.CurrentTrack.Id))
                {
                    LikeButtonText = "Like";
                    // Track Event
                    App.Telemetry.TrackEvent("Unlike Track");
                }
                else
                {
                    LikeButtonText = "Unlike";
                }
            }
            else
            {
                // Add a like to the users likes and see if successful
                if (await SoundByteService.Current.PutAsync(ServiceType.SoundCloud, $"/e1/me/track_likes/{Service.CurrentTrack.Id}"))
                {
                    LikeButtonText = "Unlike";
                    // Track Event
                    App.Telemetry.TrackEvent("Like Track");
                }
                else
                {
                    LikeButtonText = "Like";
                }
            }
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
            Service.Player.PlaybackSession.Position = comment.CommentTime;
        }

        public async void ShareTrack()
        {
            await NavigationService.Current.CallDialogAsync<ShareDialog>(Service.CurrentTrack);
        }

        /// <summary>
        ///     Navigates the user to the current track users profile
        /// </summary>
        public async void GoToUserProfile()
        {
            // Show the loading ring as loading the user can take
            // some time.
            await App.SetLoadingAsync(true);

            // We only support viewing soundcloud profiles at this time
            if (Service.CurrentTrack?.ServiceType != ServiceType.SoundCloud)
            {
                await new MessageDialog(
                    "SoundByte does not currently supporting user profiles that are not from SoundCloud.",
                    "Not Ready Yet").ShowAsync();
                return;
            }

            // Get the user object
            var currentUser = await SoundByteService.Current.GetAsync<SoundCloudUser>(ServiceType.SoundCloud, "/users/" + Service.CurrentTrack.User.Id);

            // Hide the loading ring
            await App.SetLoadingAsync(false);

            // Navigate to the user page
            App.NavigateTo(typeof(UserView), currentUser.ToBaseUser());
        }

        #endregion
    }
}