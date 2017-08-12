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

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    ///     Displays a playlist
    /// </summary>
    public sealed partial class Playlist
    {
        // Page View Model
        public PlaylistViewModel ViewModel = new PlaylistViewModel();

        public Playlist()
        {
            // Setup the XAML
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
            // Make sure the view is ready for the user
            // Track Event
            TelemetryService.Current.TrackPage("Playlist Page");
            await ViewModel.SetupView(e.Parameter as Core.API.Endpoints.Playlist);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                var cacheSize = ((Frame)Parent).CacheSize;
                ((Frame)Parent).CacheSize = 0;
                ((Frame)Parent).CacheSize = cacheSize;
            }
        }
    }
}