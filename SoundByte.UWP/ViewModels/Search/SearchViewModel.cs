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

using SoundByte.UWP.ViewModels.SearchViewModels;

namespace SoundByte.UWP.ViewModels.Search
{
    public class SearchViewModel : BaseViewModel
    {
        #region Private Variables
        // The query string
        private string _searchQuery;

        #endregion

        #region View Models
        /// <summary>
        /// View Model for tracks page
        /// </summary>
        public TracksViewModel TracksViewModel { get; } = new TracksViewModel();

        /// <summary>
        /// View model for the playlists page
        /// </summary>
        public PlaylistsViewModel PlaylistsViewModel { get; } = new PlaylistsViewModel();

        /// <summary>
        /// View model for the users page
        /// </summary>
        public UsersViewModel UsersViewModel { get; } = new UsersViewModel();

        /// <summary>
        /// View model for the podcasts page
        /// </summary>
        public PodcastsViewModel PodcastsViewModel { get; } = new PodcastsViewModel();

        #endregion

        #region Getters and Setters
        /// <summary>
        ///     The current pivot item that the user is viewing
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (value != _searchQuery)
                {
                    _searchQuery = value;
                    UpdateProperty();

                    TracksViewModel.SearchQuery = value;
                    PlaylistsViewModel.SearchQuery = value;
                    UsersViewModel.SearchQuery = value;
                    PodcastsViewModel.SearchQuery = value;
                }
            }
        }
        #endregion

        #region Method Bindings
        /// <summary>
        /// Refresh all items
        /// </summary>
        public void RefreshAll()
        {
            TracksViewModel.RefreshAll();
            PlaylistsViewModel.RefreshAll();
            UsersViewModel.RefreshAll();
            PodcastsViewModel.RefreshAll();
        }
        #endregion
    }
}