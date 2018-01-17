using Windows.UI.Xaml;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// Shows a list of shows that the user has subscribed to.
    /// </summary>
    public sealed partial class CollectionView
    {
        public CollectionView()
        {
            InitializeComponent();
        }

        private void RefreshAll(object sender, RoutedEventArgs e)
        {
            switch (Pivot.SelectedIndex)
            {
                case 0: // Likes
                    LikesView.SoundByteLikes.RefreshItems();
                    LikesView.SoundCloudLikes.RefreshItems();
                    LikesView.YouTubeLikes.RefreshItems();
                    LikesView.FanburstLikes.RefreshItems();
                    break;
                case 1: // Playlists
                    PlaylistsView.SoundBytePlaylists.RefreshItems();
                    PlaylistsView.SoundCloudPlaylists.RefreshItems();
                    PlaylistsView.YouTubePlaylists.RefreshItems();
                    PlaylistsView.FanburstPlaylists.RefreshItems();
                    break;
            }
        }
    } 
}
