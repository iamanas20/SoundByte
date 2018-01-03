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
using SoundByte.Core;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.SoundCloud.User;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Generic;

namespace SoundByte.UWP.Views.CollectionViews
{
    public sealed partial class PlaylistsView 
    {
        public SoundByteCollection<SoundCloudUserPlaylistSource, BasePlaylist> PlaylistModel { get; } =
            new SoundByteCollection<SoundCloudUserPlaylistSource, BasePlaylist>();

        public PlaylistsView()
        {
            InitializeComponent();

            PlaylistModel.Source.User = SoundByteService.Current.GetConnectedUser(ServiceType.SoundCloud);
        }

        public void NavigatePlaylist(object sender, ItemClickEventArgs e)
        {
            App.NavigateTo(typeof(PlaylistView), e.ClickedItem as BasePlaylist);
        }

        public void ViewAllSoundCloud()
        {
            App.NavigateTo(typeof(PlaylistListView), new PlaylistListViewModel.PlaylistViewModelHolder
            {
                Playlist = PlaylistModel.Source,
                Title = "Playlists"
            });
        }
    }
}
