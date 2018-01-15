using Windows.UI.Xaml.Controls;
using SoundByte.Core;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.SoundCloud.User;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Generic;

namespace SoundByte.UWP.Views.Collection
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
