using SoundByte.UWP.ViewModels.SearchViewModels;

namespace SoundByte.UWP.Views.Search
{
    public sealed partial class TracksView
    {
        public TracksView()
        {
            InitializeComponent();
        }

        public TracksViewModel ViewModel { get; set; }
    }
}