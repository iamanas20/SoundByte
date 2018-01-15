using SoundByte.UWP.ViewModels.SearchViewModels;

namespace SoundByte.UWP.Views.Search
{
    public sealed partial class PlaylistsView 
    {
        public PlaylistsView()
        {
            InitializeComponent();
        }

        public PlaylistsViewModel ViewModel { get; set; }
    }
}
