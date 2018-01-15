using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.ViewModels.Generic;

namespace SoundByte.UWP.Views.Generic
{
    public sealed partial class TrackListView 
    {
        public TrackListViewModel ViewModel { get; } = new TrackListViewModel();

        public TrackListView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init((TrackListViewModel.TrackViewModelHolder)e.Parameter);
        }      
    }
}
