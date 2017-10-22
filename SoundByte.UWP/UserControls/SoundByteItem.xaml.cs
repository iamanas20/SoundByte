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
using System.Numerics;
using Windows.UI.Composition;
using SoundByte.Core;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UICompositionAnimations.Composition;

namespace SoundByte.UWP.UserControls
{
    public sealed partial class SoundByteItem
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

            PlaybackService.Instance.OnCurrentTrackChanged += CurrentTrackChanged;
        }

        ~SoundByteItem()
        {
            PlaybackService.Instance.OnCurrentTrackChanged -= CurrentTrackChanged;
        }

        private async void CurrentTrackChanged(BaseTrack newTrack)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                if (ItemType == ItemType.Track && Track != null && TrackNowPlaying != null)
                    TrackNowPlaying.Visibility = newTrack?.Id == Track?.Id ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void SoundByteItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            UnloadObject((Grid)FindName("DesktopPlaylistItem"));
            UnloadObject((Grid)FindName("DesktopTrackItem"));
            UnloadObject((Grid)FindName("DesktopUserItem"));

            switch (ItemType)
            {
                case ItemType.Playlist:
                    // Generate and show the desktop playlist item
                    ((Grid)FindName("DesktopPlaylistItem")).Visibility = Visibility.Visible;
                    break;
                case ItemType.Track:
                        // Generate and show the desktop track item
                        ((Grid)FindName("DesktopTrackItem")).Visibility = Visibility.Visible;

                        // Update the visibilty
                        TrackNowPlaying.Visibility = PlaybackService.Instance.CurrentTrack?.Id == Track?.Id ? Visibility.Visible : Visibility.Collapsed;
                    
                    break;
                case ItemType.User:
                    // Generate and show the desktop user item
                    ((Grid)FindName("DesktopUserItem")).Visibility = Visibility.Visible;
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

        private void StartAnimations(UIElement element)
        {
            var panel = (DropShadowPanel)element;

            panel.Offset(0, -8, 250).Start();

            panel.DropShadow.StartAnimation("Offset",
                panel.DropShadow.Compositor.CreateVector3KeyFrameAnimation(new Vector3(0, 7, 0), new Vector3(0, 10, 0), TimeSpan.FromMilliseconds(250), null));

            panel.DropShadow.StartAnimation("Opacity",
                panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(0.6f, 0.8f, TimeSpan.FromMilliseconds(250), null));

            panel.DropShadow.StartAnimation("BlurRadius",
                panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(15.0f, 35.0f, TimeSpan.FromMilliseconds(250), null));
        }

        private void StopAnimation(UIElement element)
        {
            var panel = (DropShadowPanel)element;

            panel.Offset(0, 0, 350).Start();

            panel.DropShadow.StartAnimation("BlurRadius",
                panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(35.0f, 15.0f, TimeSpan.FromMilliseconds(250), null));

            panel.DropShadow.StartAnimation("Offset",
                panel.DropShadow.Compositor.CreateVector3KeyFrameAnimation(new Vector3(0, 10, 0), new Vector3(0, 7, 0), TimeSpan.FromMilliseconds(250), null));

            panel.DropShadow.StartAnimation("Opacity",
                panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(0.8f, 0.6f, TimeSpan.FromMilliseconds(250), null));
        }

        private void DesktopTrackItem_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            StartAnimations(ShadowPanel);
        }

        private void DesktopTrackItem_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            StopAnimation(ShadowPanel);
        }

        private void DesktopPlaylistItem_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            StopAnimation(PlaylistDropShadow);
        }

        private void DesktopPlaylistItem_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            StartAnimations(PlaylistDropShadow);
        }
    }
}
