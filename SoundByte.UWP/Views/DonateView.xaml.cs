using Windows.UI.Xaml.Navigation;

namespace SoundByte.UWP.Views
{
    public sealed partial class DonateView
    {
        public DonateView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.Telemetry.TrackPage("Donate View");
        }
    }
}