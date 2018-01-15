using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.ViewModels.Generic;

namespace SoundByte.UWP.Views.Generic
{
    public sealed partial class UserListView
    {
        public UserListViewModel ViewModel { get; } = new UserListViewModel();

        public UserListView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init((UserListViewModel.UserViewModelHolder)e.Parameter);
        }
    }
}
