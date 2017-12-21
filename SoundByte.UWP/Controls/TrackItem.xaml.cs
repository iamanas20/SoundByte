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
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Microsoft.Toolkit.Uwp.UI.Animations;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Services;
using UICompositionAnimations.Composition;

namespace SoundByte.UWP.Controls
{
    public sealed partial class TrackItem
    {
        /// <summary>
        /// Identifies the Track dependency property.
        /// </summary>
        public static readonly DependencyProperty TrackProperty =
            DependencyProperty.Register(nameof(Track), typeof(BaseTrack), typeof(TrackItem), null);

        /// <summary>
        /// Gets or sets the rounding interval for the Value.
        /// </summary>
        public BaseTrack Track
        {
            get => (BaseTrack)GetValue(TrackProperty);
            set => SetValue(TrackProperty, value);
        }

        public TrackItem()
        {
            InitializeComponent();
        }
 

        private async void Share(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<ShareDialog>(Track);
        }

        private async void AddToPlaylist(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<PlaylistDialog>(Track);
        }

        private async void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Offset.Y",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 10.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("Opacity",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.9f, TimeSpan.FromMilliseconds(200), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 45.0f, TimeSpan.FromMilliseconds(200), null));

            await Task.WhenAll(new List<Task>
            {
                HoverArea.Fade(1, 200).StartAsync(),
                TrackImage.Blur(15, 200).StartAsync()
            });
        }

        private async void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Offset.Y",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 6.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("Opacity",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.4f, TimeSpan.FromMilliseconds(200), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 25.0f, TimeSpan.FromMilliseconds(200), null));

            await Task.WhenAll(new List<Task>
            {
                HoverArea.Fade(0, 200).StartAsync(),
                TrackImage.Blur(0, 200).StartAsync()
            });
        }
    }
}
