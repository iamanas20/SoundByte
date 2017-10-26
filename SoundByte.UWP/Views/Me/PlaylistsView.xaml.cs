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

using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.Playlist;
using SoundByte.UWP.Services;
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

            Unloaded += (sender, args) =>
            {
                GC.Collect();
            };
        }

        /// <summary>
        ///     The playlist model that contains the users playlists / liked playlists
        /// </summary>
        private UserPlaylistModel PlaylistModel { get; } = new UserPlaylistModel();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("User Playlists View");
        }

        public void NavigatePlaylist(object sender, ItemClickEventArgs e)
        {
            StackPanel.PrepareConnectedAnimation("PlaylistImage", e.ClickedItem as BasePlaylist, "PlaylistImage");

            App.NavigateTo(typeof(PlaylistView), e.ClickedItem as BasePlaylist);
        }
    }
}