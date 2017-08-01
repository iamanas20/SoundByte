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
using SoundByte.Core.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// This page is the main landing page for any user.
    /// This page displays the users stream, the latest/trending tracks,
    /// and the users playlists/likes.
    /// </summary>
    public sealed partial class HomeView
    {
        // The view model
        public HomeViewModel ViewModel = new HomeViewModel();

        /// <summary>
        /// Setup page and init the xaml
        /// </summary>
        public HomeView()
        {
            InitializeComponent();
            // This page must be cached
            NavigationCacheMode = NavigationCacheMode.Enabled;
            // Set the data context
            DataContext = ViewModel;
            // Page has been unloaded from UI
            Unloaded += (s, e) => ViewModel.Dispose();
        }

        /// <summary>
        /// Called when the user navigates to the page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Only show the stream pivot when soundcloud account is connected
            StreamPivotItem.IsEnabled = SoundByteService.Current.IsSoundCloudAccountConnected;

            HomePivot.SelectedIndex = !SoundByteService.Current.IsSoundCloudAccountConnected ? 1 : 0;

            // Set the last visited frame (crash handling)
            SettingsService.Current.LastFrame = typeof(HomeView).FullName;
            // Store the latest time (for notification task)
            SettingsService.Current.LatestViewedTrack = DateTime.Now;
            // Track Event
            TelemetryService.Current.TrackPage("Home Page");
        }
    }
}