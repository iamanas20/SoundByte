using Windows.UI.Xaml.Navigation;
using SoundByte.Core;
using SoundByte.Core.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.SoundCloud
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
            if (!SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud))
                App.NavigateTo(typeof(ExploreView));

            // Track Event
            App.Telemetry.TrackPage("SoundCloud Stream View");
        }
    }
}