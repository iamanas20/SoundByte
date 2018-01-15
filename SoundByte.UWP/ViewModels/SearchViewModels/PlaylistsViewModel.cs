using Windows.UI.Xaml.Controls;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Sources.SoundCloud.Search;
using SoundByte.Core.Sources.YouTube.Search;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views;
using SoundByte.UWP.Views.Generic;

namespace SoundByte.UWP.ViewModels.SearchViewModels
{
    public class PlaylistsViewModel : BaseViewModel
    {
        #region Sources
        /// <summary>
        /// Model for SoundCloud playlists
        /// </summary>
        public SoundByteCollection<SoundCloudSearchPlaylistSource, BasePlaylist> SoundCloudPlaylists { get; } =
            new SoundByteCollection<SoundCloudSearchPlaylistSource, BasePlaylist>();

        /// <summary>
        /// Model for YouTube playlists
        /// </summary>
        public SoundByteCollection<YouTubeSearchPlaylistSource, BasePlaylist> YouTubePlaylists { get; } =
            new SoundByteCollection<YouTubeSearchPlaylistSource, BasePlaylist>();
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
                SoundCloudPlaylists.Source.SearchQuery = value;
                SoundCloudPlaylists.RefreshItems();

                YouTubePlaylists.Source.SearchQuery = value;
                YouTubePlaylists.RefreshItems();
            }
        }
        #endregion

        #region Refresh Logic
        public void RefreshAll()
        {
            SoundCloudPlaylists.RefreshItems();
            YouTubePlaylists.RefreshItems();
        }
        #endregion

        #region View All
        public void NavigateSoundCloudPlaylists()
        {
            App.NavigateTo(typeof(PlaylistListView), new PlaylistListViewModel.PlaylistViewModelHolder
            {
                Playlist = SoundCloudPlaylists.Source,
                Title = $"Results for \"{SearchQuery}\""
            });
        }

        public void NavigateYouTubePlaylists()
        {
            App.NavigateTo(typeof(PlaylistListView), new PlaylistListViewModel.PlaylistViewModelHolder
            {
                Playlist = YouTubePlaylists.Source,
                Title = $"Results for \"{SearchQuery}\""
            });
        }
        #endregion

        #region ViewSingle
        public void NavigatePlaylist(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            App.NavigateTo(typeof(PlaylistView), e.ClickedItem as BasePlaylist);
        }
        #endregion
    }
}
