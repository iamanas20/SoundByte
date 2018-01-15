using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.ViewModels.Generic;

namespace SoundByte.UWP.Views.Generic
{
    public sealed partial class PlaylistListView
    {
        public PlaylistListViewModel ViewModel { get; } = new PlaylistListViewModel();

        public PlaylistListView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init((PlaylistListViewModel.PlaylistViewModelHolder)e.Parameter);
        }
    }
}