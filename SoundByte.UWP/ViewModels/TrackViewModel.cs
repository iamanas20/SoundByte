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
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.UWP.Models;
using SoundByte.Core.API.Endpoints;
using SoundByte.Core.Converters;
using SoundByte.Core.Helpers;
using SoundByte.Core.Services;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.ViewModels
{
    public class TrackViewModel : BaseViewModel
    {
        #region Private Variables
        // The comment text box
        private string _commentText;
        // Timer to update the front page UI
        private DispatcherTimer _pageTimer;
        // Used to block the timer when interacting with the slider value
        private bool _blockPageTimer;
        // The pin button text
        private string _pinButtonText = "Pin";
        // The like button text
        private string _likeButtonText = "Like";
        // The repost button text
        private string _repostButtonText = "Repost";
        // The repeat button style
        private Windows.UI.Text.FontWeight _repeatButtonWeight = new Windows.UI.Text.FontWeight { Weight = 400};
        // The shuffle button style
        private Windows.UI.Text.FontWeight _shuffleButtonWeight = new Windows.UI.Text.FontWeight { Weight = 400 };
        #endregion

        #region Models
        // Model for the comments
        public CommentModel CommentItems { get; } = new CommentModel(PlaybackService.Current.CurrentTrack);
        #endregion

        #region Getters and Setters

        /// <summary>
        /// The repeat button weight
        /// </summary>
        public Windows.UI.Text.FontWeight RepeatButtonWeight
        {
            get => _repeatButtonWeight;
            set
            {
                _repeatButtonWeight = value;
                UpdateProperty();
            }
        }

        /// <summary>
        /// The shuffle button weight
        /// </summary>
        public Windows.UI.Text.FontWeight ShuffleButtonWeight
        {
            get => _shuffleButtonWeight;
            set
            {
                _shuffleButtonWeight = value;
                UpdateProperty();
            }
        }

       

        /// <summary>
        /// The text on the pin button
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
        /// The text on the like button
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
        /// The text on the repost button
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
        /// The comment text for the comment box
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
        /// Setup the view model
        /// </summary>
        public void SetupModel()
        {
            // Bind the event handlers   
            // Timer Setup
            _pageTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            // Setup the tick event
            _pageTimer.Tick += PlayingSliderUpdate;

            // If the timer is ready, start it
            if (!_pageTimer.IsEnabled)
                _pageTimer.Start();

            // Bind the method once we know a playback list exists
            if (PlaybackService.Current.PlaybackList != null)
                PlaybackService.Current.PlaybackList.CurrentItemChanged += CurrentItemChanged;
        }

        public void MakeFullScreen()
        {
            if (!DeviceHelper.IsDeviceFullScreen)
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            else
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
        }

        /// <summary>
        /// Clean the view model
        /// </summary>
        public void CleanModel()
        {
            // Stop the timer
            _pageTimer.Stop();

            // Unbind the events
            if(PlaybackService.Current.PlaybackList != null)
                PlaybackService.Current.PlaybackList.CurrentItemChanged -= CurrentItemChanged;

            _pageTimer.Tick -= PlayingSliderUpdate;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the current playing item changes
        /// </summary>
        private async void CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Only perform the following actions if there is a new track
                if (args.NewItem == null)
                    return;

                if (Service.CurrentTrack == null)
                    return;

                // Set the pin button text
                PinButtonText = TileService.Current.DoesTileExist("Track_" + Service.CurrentTrack.Id) ? "Unpin" : "Pin";

                // Set the like button text
                LikeButtonText = await SoundByteService.Current.ExistsAsync("/me/favorites/" + Service.CurrentTrack.Id) ? "Unlike" : "Like";

                // Set the repost button text
                RepostButtonText = await SoundByteService.Current.ExistsAsync("/e1/me/track_reposts/" + Service.CurrentTrack.Id) ? "Unpost" : "Repost";

                // Reload all the comments
                CommentItems.RefreshItems();

                // Create a image for the jumplist
                var tempImage = await ImageHelper.CreateCachedImageAsync(ArtworkConverter.ConvertObjectToImage(Service.CurrentTrack), "Jumplist_" + Service.CurrentTrack.Id);
                // Add the track to the jumplist
                if (tempImage != null)
                    await JumplistHelper.AddRecentAsync("soundbyte://core/track?id=" + Service.CurrentTrack.Id, Service.CurrentTrack.Title, "Play " + Service.CurrentTrack.Title + " by " + Service.CurrentTrack.User.Username + ".", "Recent Plays", tempImage);
            });
        }

        /// <summary>
        /// Timer method that is run to make sure the UI is kept up to date
        /// </summary>
        private async void PlayingSliderUpdate(object sender, object e)
        {
            // If the update is blocked, do not run this time
            if (_blockPageTimer)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return;
            }   

            // Only call the following if the player exists, is playing
            // and the time is greater then 0.
            if (PlaybackService.Current.Player == null || 
                PlaybackService.Current.Player.PlaybackSession.PlaybackState != MediaPlaybackState.Playing || 
                PlaybackService.Current.Player.PlaybackSession.Position.Milliseconds <= 0)
                return;
        }
        #endregion

        #region Method Bindings

        /// <summary>
        /// Opens the share track UI if it exists
        /// </summary>
        public  async void ShareTrack() => await new Dialogs.ShareDialog(Service.CurrentTrack).ShowAsync();

        /// <summary>
        /// Display the playlist picker if it exists
        /// </summary>
        public async void DisplayPlaylist() => await new Dialogs.PlaylistDialog(Service.CurrentTrack).ShowAsync();

        /// <summary>
        /// Toggle if we should shuffle the playlist
        /// </summary>
        public void ToggleShuffle()
        {
            // Switch the shuffle functionality
            PlaybackService.Current.PlaybackList.ShuffleEnabled = !PlaybackService.Current.PlaybackList.ShuffleEnabled;
            // If the track should shuffle
            ShuffleButtonWeight = PlaybackService.Current.PlaybackList.ShuffleEnabled ? new Windows.UI.Text.FontWeight { Weight = 600 } : new Windows.UI.Text.FontWeight { Weight = 400 };
            // Track Event
            TelemetryService.Current.TrackEvent("Toggle Shuffle");
        }

        /// <summary>
        /// Toggle if we should repeat the current track
        /// </summary>
        public void ToggleRepeat()
        {
            // Switch the repeat functionality
            PlaybackService.Current.Player.IsLoopingEnabled = !PlaybackService.Current.Player.IsLoopingEnabled;
            // If the track should repeat
            RepeatButtonWeight = PlaybackService.Current.Player.IsLoopingEnabled ? new Windows.UI.Text.FontWeight { Weight = 600 } : new Windows.UI.Text.FontWeight { Weight = 400 };
            // Track Event
            TelemetryService.Current.TrackEvent("Toggle Repeat");
        }

        /// <summary>
        /// Navigate to the selected track in the playlist
        /// </summary>
        public async void GotoRelatedTrack(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Current.StartMediaPlayback(PlaybackService.Current.Playlist.ToList(), PlaybackService.Current.TokenValue, false, (Track)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing related track.").ShowAsync();
        } 

        /// <summary>
        /// Pin the tile to the start menu
        /// </summary>
        public async void PinTile()
        {
            // Check if the tile exists
            var tileExists = TileService.Current.DoesTileExist("Track_" + Service.CurrentTrack.Id);

            if (tileExists)
            {
                // Remove the tile and check if it was successful
                if (await TileService.Current.RemoveAsync("Track_" + Service.CurrentTrack.Id))
                {
                    PinButtonText = "Pin";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Unpin Track");
                }
                else
                {
                    PinButtonText = "Unpin";
                }
            }
            else
            {
                // Create a live tile and check if it was created
                if (await TileService.Current.CreateTileAsync("Track_" + Service.CurrentTrack.Id, Service.CurrentTrack.Title, "soundbyte://core/track?id=" + Service.CurrentTrack.Id, new Uri(ArtworkConverter.ConvertObjectToImage(Service.CurrentTrack)), ForegroundText.Light))
                {
                    PinButtonText = "Unpin";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Pin Track");
                }
                else
                {
                    PinButtonText = "Pin";
                }
            }
        }

        /// <summary>
        /// Repost the current track to the users stream
        /// </summary>
        public async void RepostTrack()
        {
            if (Service.CurrentTrack == null)
                return;

            // Check to see what the existing string is
            if (RepostButtonText == "Unpost")
            {
                // Delete the repost value and check if it was successful
                if (await SoundByteService.Current.DeleteAsync("/e1/me/track_reposts/" + Service.CurrentTrack.Id))
                {
                    RepostButtonText = "Repost";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Unpost Track");
                }
                else
                {
                    RepostButtonText = "Unpost";
                }
            }
            else
            {
                // Put a value in the reposted tracks and see if successful
                if (await SoundByteService.Current.PutAsync("/e1/me/track_reposts/" + Service.CurrentTrack.Id))
                {
                    RepostButtonText = "Unpost";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Repost Track");
                }
                else
                {
                    RepostButtonText = "Repost";
                }
            }
        }

        /// <summary>
        /// Like the current track
        /// </summary>
        public async void LikeTrack()
        { 
            if (Service.CurrentTrack == null)
                return;

            // Check to see what the existing string is
            if (LikeButtonText == "Unlike")
            {
                // Delete the like from the users likes and see if successful
                if (await SoundByteService.Current.DeleteAsync("/e1/me/track_likes/" + Service.CurrentTrack.Id))
                {
                    LikeButtonText = "Like";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Unlike Track");
                }
                else
                {
                    LikeButtonText = "Unlike";
                }
            }
            else
            {
                // Add a like to the users likes and see if successful
                if (await SoundByteService.Current.PutAsync("/e1/me/track_likes/" + Service.CurrentTrack.Id))
                {
                    LikeButtonText = "Unlike";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Like Track");
                }
                else
                {
                    LikeButtonText = "Like";
                }
            }
        }

        /// <summary>
        /// Called when the user adjusts the playing slider
        /// </summary>
        public async void PlayingSliderChange()
        {
            // Block timer updates
            _blockPageTimer = true;

            // Set the track position
            Service.PlayingSliderChange();
            // Delay the unblock
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Unblock timer update
            _blockPageTimer = false;
        }

        /// <summary>
        /// Called when the user taps on a comment.
        /// This method changes the current position of the track
        /// to the comment time.
        /// </summary>
        public void GoToCommentItemPosition(object sender, ItemClickEventArgs e)
        {
            // Get the comment object
            var comment = e.ClickedItem as Comment;
            // Set the current position
            Service.Player.PlaybackSession.Position = TimeSpan.FromMilliseconds(int.Parse(comment?.Timestamp));
        }

        /// <summary>
        /// Navigates the user to the current track users profile
        /// </summary>
        public async void GoToUserProfile()
        {
            // Show the loading ring as loading the user can take
            // some time.
            App.IsLoading = true;

            // We only support viewing soundcloud profiles at this time
            if (Service.CurrentTrack.ServiceType != ServiceType.SoundCloud)
            {
                await new MessageDialog("SoundByte does not currently supporting user profiles that are not from SoundCloud.", "Not Ready Yet").ShowAsync();
                return;
            }

            // Get the user object
            var currentUser = await SoundByteService.Current.GetAsync<User>("/users/" + Service.CurrentTrack.User.Id);

            // Hide the loading ring
            App.IsLoading = false;

            // Navigate to the user page
            App.NavigateTo(typeof(Views.UserView), currentUser);
        }

        /// <summary>
        /// Post a comment
        /// </summary>
        public async void PostComment()
        {
            // Only perform this action if the user has typed something
            if (string.IsNullOrEmpty(CommentText))
                return;

            // Create a new comment object
            var comment = new Comment
            {
                Body = CommentText,
                Timestamp = PlaybackService.Current.Player.PlaybackSession.Position.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
                User = SoundByteService.Current.CurrentUser
            };

            // Clear the comment box
            CommentText = string.Empty;


            // Create an http client to post the comment
            using (var client = new HttpClient())
            {
                // Create the json for the comment
                var json = "{\"comment\": {\"body\":\"" + comment.Body + "\", \"timestamp\": " + comment.Timestamp + "}}";

                // Request the soundcloud API
                var commentResponse =  await client.PostAsync("https://api.soundcloud.com/tracks/" + Service.CurrentTrack.Id + "/comments.json?oauth_token=" + SoundByteService.Current.SoundCloudToken.AccessToken + "&client_secret=" + ApiKeyService.SoundCloudClientSecret + "&client_id" + ApiKeyService.SoundCloudClientId, new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json"));

                // If we did not post the comment, return
                if (!commentResponse.IsSuccessStatusCode) return;

                // Track the event
                TelemetryService.Current.TrackEvent("Post Comment");

                // Insert into the list
                CommentItems.Insert(0, comment);
            }
        }

        #endregion

        #region Dispose Handlers

        public override void Dispose()
        {
            // Only clean if we are in the background
            if (!App.IsBackground)
                return;

            CleanModel();

            CommentItems.RefreshItems();

            GC.Collect();
        }
        #endregion
    }
}
