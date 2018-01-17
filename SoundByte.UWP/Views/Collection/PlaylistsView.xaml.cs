using Windows.UI.Xaml.Controls;
using SoundByte.Core;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.Fanburst;
using SoundByte.Core.Sources.SoundByte;
using SoundByte.Core.Sources.SoundCloud.User;
using SoundByte.Core.Sources.YouTube;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Generic;

namespace SoundByte.UWP.Views.Collection
{
    public sealed partial class PlaylistsView 
    {
        public PlaylistsView()
        {
            InitializeComponent();

            SoundCloudPlaylists.Source.User = SoundByteService.Current.GetConnectedUser(ServiceType.SoundCloud);
        }

        #region SoundByte
        /// <summary>
        ///     Collection and model for showing SoundByte playlists.
        /// </summary>
        public SoundByteCollection<SoundBytePlaylistSource, BasePlaylist> SoundBytePlaylists { get; } =
            new SoundByteCollection<SoundBytePlaylistSource, BasePlaylist>();

        /// <summary>
        ///     Navigate to see all SoundCloud playlists.
        /// </summary>
        public void ViewAllSoundByte()
        {
            App.NavigateTo(typeof(PlaylistListView), new PlaylistListViewModel.PlaylistViewModelHolder
            {
                Playlist = SoundBytePlaylists.Source,
                Title = "Playlists"
            });
        }
        #endregion

        #region SoundCloud
        /// <summary>
        ///     Collection and model for showing SoundCloud playlists.
        /// </summary>
        public SoundByteCollection<SoundCloudUserPlaylistSource, BasePlaylist> SoundCloudPlaylists { get; } =
            new SoundByteCollection<SoundCloudUserPlaylistSource, BasePlaylist>();

        /// <summary>
        ///     Navigate to see all SoundCloud playlists.
        /// </summary>
        public void ViewAllSoundCloud()
        {
            App.NavigateTo(typeof(PlaylistListView), new PlaylistListViewModel.PlaylistViewModelHolder
            {
                Playlist = SoundCloudPlaylists.Source,
                Title = "Playlists"
            });
        }
        #endregion

        #region YouTube
        /// <summary>
        ///     Collection and model for showing YouTube playlists.
        /// </summary>
        public SoundByteCollection<YouTubePlaylistSource, BasePlaylist> YouTubePlaylists { get; } =
            new SoundByteCollection<YouTubePlaylistSource, BasePlaylist>();

        /// <summary>
        ///     Navigate to see all YouTube playlists.
        /// </summary>
        public void ViewAllYouTube()
        {
            App.NavigateTo(typeof(PlaylistListView), new PlaylistListViewModel.PlaylistViewModelHolder
            {
                Playlist = YouTubePlaylists.Source,
                Title = "Playlists"
            });
        }
        #endregion

        #region Fanburst
        /// <summary>
        ///     Collection and model for showing Fanburst playlists.
        /// </summary>
        public SoundByteCollection<FanburstPlaylistSource, BasePlaylist> FanburstPlaylists { get; } =
            new SoundByteCollection<FanburstPlaylistSource, BasePlaylist>();

        /// <summary>
        ///     Navigate to see all Fanburst playlists.
        /// </summary>
        public void ViewAllFanburst()
        {
            App.NavigateTo(typeof(PlaylistListView), new PlaylistListViewModel.PlaylistViewModelHolder
            {
                Playlist = FanburstPlaylists.Source,
                Title = "Playlists"
            });
        }
        #endregion

        public void NavigatePlaylist(object sender, ItemClickEventArgs e)
        {
            App.NavigateTo(typeof(PlaylistView), e.ClickedItem as BasePlaylist);
        }
    }
}