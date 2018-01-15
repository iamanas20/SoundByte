using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.User;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;
using WinRTXamlToolkit.Controls.Extensions;

namespace SoundByte.UWP.Views
{
    /// <summary>
    ///     Displays user information such as followers/followings, sounds, sets, likes etc.
    /// </summary>
    public sealed partial class UserView
    {
        /// <summary>
        ///     Setup XAML page
        /// </summary>
        public UserView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The main view model for this page
        /// </summary>
        public UserViewModel ViewModel { get; } = new UserViewModel();

        /// <summary>
        ///     Called when the user navigates to the page
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get the target user (may be null)
            var targetUser = e.Parameter as BaseUser;

            // If we have both objects and they equal, do
            // nothing and return (we are navigating to the
            // same page.
            if (ViewModel.User?.UserId == targetUser?.UserId)
                return;

            // If both of these are null, we have a problem.
            // In the future we would try load the user ID from
            // a stored file. For now through an exception.
            if (targetUser == null && ViewModel.User == null)
                throw new ArgumentNullException(nameof(e),
                    "Both the view model and target user are null. UserView cannot continue");

            // We need to handle window resize change events here for the
            // main pivot
            MainPivot.Height = Window.Current.Bounds.Height;
            SizeChanged += UserView_SizeChanged;

            // If the target user is not null, we can setup the 
            // the view model.
            if (targetUser != null)
            {
                // Reset the selected page for the pivot
                MainPivot.SelectedIndex = 0;

                // Clear description
                Description.Blocks.Clear();

                // Create the model
                await ViewModel.UpdateModel(targetUser);

                if (!string.IsNullOrEmpty(ViewModel.User.Description))
                {
                    TextHelper.ConvertTextToFormattedTextBlock(ViewModel.User.Description, ref Description);
                }
            }

            // Track Event
            App.Telemetry.TrackPage("User View");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SizeChanged -= UserView_SizeChanged;
        }

        private void UserView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (PlaybackService.Instance.GetCurrentTrack() == null)
                MainPivot.Height = Window.Current.Bounds.Height;
            else
                MainPivot.Height = Window.Current.Bounds.Height - 64;
        }


        private void GridViewScrollViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var v = (ScrollViewer) sender;

            if (v.VerticalOffset <= 0)
                v.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        private void MainScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var v = (ScrollViewer) sender;

            // When we are at the bottom
            if (v.ScrollableHeight < 0 || (int) v.VerticalOffset == (int) v.ScrollableHeight)
                switch (ViewModel.SelectedPivotItem.Name)
                {
                    case "TracksPivot":
                        TracksView.GetScrollViewer().VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        break;

                    case "LikesPivot":
                        LikesView.GetScrollViewer().VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        break;

                    case "PlaylistsPivot":
                        PlaylistsView.GetScrollViewer().VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        break;

                    case "FollowersPivot":
                        FollowersView.GetScrollViewer().VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        break;

                    case "FollowingsPivot":
                        FollowingsView.GetScrollViewer().VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        break;
                }
        }

        private void MainPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}