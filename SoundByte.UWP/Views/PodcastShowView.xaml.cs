using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.Podcast;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    public sealed partial class PodcastShowView
    {
        public PodcastShowViewModel ViewModel { get; } = new PodcastShowViewModel();

        public PodcastShowView()
        {
            InitializeComponent();

            Unloaded += (s, e) =>
            {
                ViewModel.Dispose();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init(e.Parameter as BasePodcast);
        }
    }
}