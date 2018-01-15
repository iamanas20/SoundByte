/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
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
using SoundByte.Core.Sources.Fanburst.Search;
using SoundByte.Core.Sources.SoundCloud.Search;
using SoundByte.Core.Sources.YouTube;
using SoundByte.Core.Sources.YouTube.Search;
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
        public SoundByteCollection<SoundCloudSearchUserSource, BaseUser> SoundCloudUsers { get; } =
            new SoundByteCollection<SoundCloudSearchUserSource, BaseUser>();

        /// <summary>
        /// Model for YouTube users
        /// </summary>
        public SoundByteCollection<YouTubeSearchUserSource, BaseUser> YouTubeUsers { get; } =
            new SoundByteCollection<YouTubeSearchUserSource, BaseUser>();

        /// <summary>
        /// Model for Fanburst users
        /// </summary>
        public SoundByteCollection<FanburstSearchUserSource, BaseUser> FanburstUsers { get; } =
            new SoundByteCollection<FanburstSearchUserSource, BaseUser>();
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

                YouTubeUsers.Source.SearchQuery = value;
                YouTubeUsers.RefreshItems();

                FanburstUsers.Source.SearchQuery = value;
                FanburstUsers.RefreshItems();
            }
        }
        #endregion

        #region Refresh Logic
        public void RefreshAll()
        {
            SoundCloudUsers.RefreshItems();
            YouTubeUsers.RefreshItems();
            FanburstUsers.RefreshItems();
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

        public void NavigateYouTubeUsers()
        {
            App.NavigateTo(typeof(UserListView), new UserListViewModel.UserViewModelHolder
            {
                User = YouTubeUsers.Source,
                Title = $"Results for \"{SearchQuery}\""
            });
        }

        public void NavigateFanburstUsers()
        {
            App.NavigateTo(typeof(UserListView), new UserListViewModel.UserViewModelHolder
            {
                User = FanburstUsers.Source,
                Title = $"Results for \"{SearchQuery}\""
            });
        }
        #endregion

        #region ViewSingle
        public void NavigateUser(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            App.NavigateTo(typeof(UserView), e.ClickedItem as BaseUser);
        }
        #endregion
    }
}
