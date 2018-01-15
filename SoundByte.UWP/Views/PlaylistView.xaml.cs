using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.Playlist;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    ///     Displays a playlist
    /// </summary>
    public sealed partial class PlaylistView
    {
        // Page View Model
        public PlaylistViewModel ViewModel = new PlaylistViewModel();

        public PlaylistView()
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
          //  var imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("PlaylistImage");
         //   imageAnimation?.TryStart(PlaylistImageHolder, new[] { TitlePanel });

            // Make sure the view is ready for the user
            // Track Event
            App.Telemetry.TrackPage("Playlist View");
            await ViewModel.SetupView((BasePlaylist)e.Parameter);
        }
    }
}