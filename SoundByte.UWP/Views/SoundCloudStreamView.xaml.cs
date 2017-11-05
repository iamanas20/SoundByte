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

using Windows.UI.Xaml.Navigation;
using SoundByte.Core;
using SoundByte.Core.Services;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    ///     This page is the main landing page for any user.
    ///     This page displays the users stream, the latest/trending tracks,
    ///     and the users playlists/likes.
    /// </summary>
    public sealed partial class SoundCloudStreamView
    {
        // The view model
        public SoundCloudStreamViewModel ViewModel { get; } = new SoundCloudStreamViewModel();

        /// <summary>
        ///     Setup page and init the xaml
        /// </summary>
        public SoundCloudStreamView()
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud))
                App.NavigateTo(typeof(ExploreView));

            // Track Event
            TelemetryService.Instance.TrackPage("SoundCloud Stream View");
        }
    }
}