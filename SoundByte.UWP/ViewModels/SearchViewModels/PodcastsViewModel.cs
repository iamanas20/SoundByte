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

using SoundByte.Core.Items;
using SoundByte.Core.Sources;
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.ViewModels.SearchViewModels
{
    public class PodcastsViewModel : BaseViewModel
    {
        #region Sources
        /// <summary>
        /// Model for podcasts
        /// </summary>
        public SoundByteCollection<SearchPodcastSource, PodcastShow> Podcasts { get; } =
            new SoundByteCollection<SearchPodcastSource, PodcastShow>();
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
                Podcasts.Source.SearchQuery = value;
                Podcasts.RefreshItems();
            }
        }
        #endregion

        #region Refresh Logic
        public void RefreshAll()
        {
            Podcasts.RefreshItems();
        }
        #endregion
    }
}
