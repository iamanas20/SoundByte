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
using Windows.UI;
using SoundByte.Core;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
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
        }


        private void SoundByteItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            switch (ItemType)
            {
                case ItemType.Playlist:
                    // Generate and show the desktop playlist item
                    FindName("DesktopPlaylistItem");
                    DesktopPlaylistItem.Visibility = Visibility.Visible;
                    break;
                case ItemType.Track:
                    // Generate and show the desktop track item
                    FindName("DesktopTrackItem");
                    DesktopTrackItem.Visibility = Visibility.Visible;
                    break;
                case ItemType.User:
                    // Generate and show the desktop user item
                    DesktopUserItem.Visibility = Visibility.Visible;
                    break;
            }
        }

        public static void StartAnimations(UIElement element, Windows.UI.Color hoverColor)
        {
            var panel = (DropShadowPanel)element;

            panel.Offset(0, -3, 250).Start();

            panel.DropShadow.StartAnimation("Offset.Y",
                      panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 10.0f, TimeSpan.FromMilliseconds(250), null));

            panel.DropShadow.StartAnimation("Opacity",
                panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.9f, TimeSpan.FromMilliseconds(250), null));

            panel.DropShadow.StartAnimation("BlurRadius",
                panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 45.0f, TimeSpan.FromMilliseconds(250), null));

            var colorAnimation = panel.DropShadow.Compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = TimeSpan.FromMilliseconds(250);

            colorAnimation.InsertKeyFrame(0.0f, Colors.Black);
            colorAnimation.InsertKeyFrame(1.0f, hoverColor);

            panel.DropShadow.StartAnimation("Color", colorAnimation);   
        }

        public static void StopAnimation(UIElement element)
        {
            var panel = (DropShadowPanel)element;

            panel.Offset(0, 0, 250).Start();

               panel.DropShadow.StartAnimation("Offset.Y",
                   panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 6.0f, TimeSpan.FromMilliseconds(250), null));

            panel.DropShadow.StartAnimation("Opacity",
                panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.4f, TimeSpan.FromMilliseconds(250), null));

            panel.DropShadow.StartAnimation("BlurRadius",
                panel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 25.0f, TimeSpan.FromMilliseconds(250), null));

            var colorAnimation = panel.DropShadow.Compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = TimeSpan.FromMilliseconds(250);

            colorAnimation.InsertKeyFrame(1.0f, Colors.Black);

            panel.DropShadow.StartAnimation("Color", colorAnimation);
        }    

        private void DesktopUserItem_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            StartAnimations(UserDropShadow, Colors.Black);
        }

        private void DesktopUserItem_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            StopAnimation(UserDropShadow);
        }
    }
}