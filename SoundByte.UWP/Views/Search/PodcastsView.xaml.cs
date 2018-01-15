using SoundByte.UWP.ViewModels.SearchViewModels;

namespace SoundByte.UWP.Views.Search
{
    public sealed partial class PodcastsView 
    {
        public PodcastsView()
        {
            InitializeComponent();
        }

        public PodcastsViewModel ViewModel { get; set; }
    }
}
