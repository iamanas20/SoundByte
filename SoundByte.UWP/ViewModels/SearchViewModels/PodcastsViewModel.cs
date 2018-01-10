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
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Sources.Podcast;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.ViewModels.SearchViewModels
{
    public class PodcastsViewModel : BaseViewModel
    {
        #region Sources
        /// <summary>
        /// Model for podcasts
        /// </summary>
        public SoundByteCollection<PodcastSearchSource, PodcastShow> Podcasts { get; } =
            new SoundByteCollection<PodcastSearchSource, PodcastShow>();
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

        #region View Single
        public void NavigatePodcastShow(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            App.NavigateTo(typeof(PodcastShowView), e.ClickedItem as PodcastShow);
        }
        #endregion
    }
}
