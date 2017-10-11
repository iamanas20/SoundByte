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

using SoundByte.Core;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.UWP.UserControls
{
    public sealed partial class SoundByteItem : UserControl
    {
        public static readonly DependencyProperty ItemTypeProperty =
           DependencyProperty.Register("ItemType", typeof(ItemType), typeof(SoundByteItem), null);

        /// <summary>
        /// The type of item this control should display
        /// </summary>
        public ItemType ItemType
        {
            get
            {
                var val = GetValue(ItemTypeProperty);

                if (val == null)
                    return ItemType.Unknown;
                else
                    return (ItemType)GetValue(ItemTypeProperty);
            }
            set => SetValue(ItemTypeProperty, value);
        }

        public static readonly DependencyProperty UserProperty =
           DependencyProperty.Register("User", typeof(BaseUser), typeof(SoundByteItem), null);

        /// <summary>
        /// User Item
        /// </summary>
        public BaseUser User
        {
            get => (BaseUser) GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public static readonly DependencyProperty TrackProperty =
           DependencyProperty.Register("Track", typeof(BaseTrack), typeof(SoundByteItem), null);

        /// <summary>
        /// Track item
        /// </summary>
        public BaseTrack Track
        {
            get => (BaseTrack) GetValue(TrackProperty);
            set => SetValue(TrackProperty, value);
        }

        public static readonly DependencyProperty PlaylistProperty =
           DependencyProperty.Register("Playlist", typeof(BasePlaylist), typeof(SoundByteItem), null);

        /// <summary>
        /// Playlist item
        /// </summary>
        public BasePlaylist Playlist
        {
            get => (BasePlaylist) GetValue(PlaylistProperty);
            set => SetValue(PlaylistProperty, value);
        }

        public SoundByteItem()
        {
            InitializeComponent();

            DataContextChanged += SoundByteItem_DataContextChanged;
        }

        private void SoundByteItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            switch(ItemType)
            {
                case ItemType.Playlist:
                    // Generate and show the desktop playlist item
                    ((Grid)FindName("DesktopPlaylistItem")).Visibility = Visibility.Visible;
                    break;
                case ItemType.Track:
                    // Generate and show the desktop track item
                    ((Grid)FindName("DesktopTrackItem")).Visibility = Visibility.Visible;
                    break;
                case ItemType.User:
                    break;
            }
        }

        private async void ShareTrack(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<ShareDialog>(Track);
        }

        private async void AddTrackToPlaylist(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<PlaylistDialog>(Track);
        }
    }
}
