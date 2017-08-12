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
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Services;
using SoundByte.UWP.Models;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    ///     Let the user view their playlists
    /// </summary>
    public sealed partial class PlaylistsView
    {
        public PlaylistsView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The playlist model that contains the users playlists / liked playlists
        /// </summary>
        private UserPlaylistModel PlaylistModel { get; } = new UserPlaylistModel();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("User Playlist Page");
        }

        public void NavigatePlaylist(object sender, ItemClickEventArgs e)
        {
            App.NavigateTo(typeof(Playlist), e.ClickedItem as Core.API.Endpoints.Playlist);
        }
    }
}