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

using Windows.UI.Xaml.Controls;
using SoundByte.Core.Items.User;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views;
using SoundByte.UWP.Views.Generic;

namespace SoundByte.UWP.ViewModels.SearchViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        #region Sources
        /// <summary>
        /// Model for SoundCloud users
        /// </summary>
        public SoundByteCollection<SearchSoundCloudUserSource, BaseUser> SoundCloudUsers { get; } =
            new SoundByteCollection<SearchSoundCloudUserSource, BaseUser>();
        #endregion

        #region Private Variables
        // The query string
        private string _searchQuery;
        #endregion

        #region Getters and Setters
        /// <summary>
        /// The current search query, setting this value will cause
        /// the sources to update.
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (value == _searchQuery) return;

                _searchQuery = value;
                UpdateProperty();

                // Update the models
                SoundCloudUsers.Source.SearchQuery = value;
                SoundCloudUsers.RefreshItems();
            }
        }
        #endregion

        #region Refresh Logic
        public void RefreshAll()
        {
            SoundCloudUsers.RefreshItems();
        }
        #endregion

        #region View All
        public void NavigateSoundCloudUsers()
        {
            App.NavigateTo(typeof(UserListView), new UserListViewModel.UserViewModelHolder
            {
                User = SoundCloudUsers.Source,
                Title = $"Results for \"{SearchQuery}\""
            });
        }
        #endregion

        #region ViewSingle
        public void NavigateSoundCloudUser(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            App.NavigateTo(typeof(UserView), e.ClickedItem as BaseUser);
        }
        #endregion
    }
}
