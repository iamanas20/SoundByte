/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

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
