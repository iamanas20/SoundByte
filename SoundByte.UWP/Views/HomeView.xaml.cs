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
using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    ///     This page is the main landing page for any user.
    ///     This page displays the users stream, the latest/trending tracks,
    ///     and the users playlists/likes.
    /// </summary>
    public sealed partial class HomeView
    {
        // The view model
        public HomeViewModel ViewModel = new HomeViewModel();

        /// <summary>
        ///     Setup page and init the xaml
        /// </summary>
        public HomeView()
        {
            InitializeComponent();
            // Set the data context
            DataContext = ViewModel;

            Unloaded += (s, e) =>
            {
                ViewModel.Dispose();
            };
        }

        /// <summary>
        ///     Called when the user navigates to the page
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Only show the stream pivot when soundcloud account is connected
            StreamPivotItem.IsEnabled = SoundByteService.Instance.IsSoundCloudAccountConnected;

            // Bootstart the user stream
            if (SoundByteService.Instance.IsSoundCloudAccountConnected && ViewModel.StreamItems.Count == 0)
                await ViewModel.StreamItems.LoadMoreItemsAsync(5);

            HomePivot.SelectedIndex = !SoundByteService.Instance.IsSoundCloudAccountConnected ? 1 : 0;

            // Track Event
            TelemetryService.Instance.TrackPage("Home View");
        }
    }
}