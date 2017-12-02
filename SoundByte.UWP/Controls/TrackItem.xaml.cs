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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SoundByte.Core.Items.Track;
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

        private Color _hoverColor = Colors.Black;

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

            Loaded += (s, e) =>
            {
                PlaybackService.Instance.OnCurrentTrackChanged += CurrentTrackChanged;

            };

            Unloaded += (s, e) =>
            {
                PlaybackService.Instance.OnCurrentTrackChanged -= CurrentTrackChanged;

            };
        }

        private async void CurrentTrackChanged(BaseTrack newTrack)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                if (newTrack?.Id == Track?.Id)
                {
                    FindName("TrackNowPlaying");
                }
                else
                {
                    UnloadObject(TrackNowPlaying);
                }
            });
        }

        private void Share(object sender, RoutedEventArgs e)
        {

        }

        private void AddToPlaylist(object sender, RoutedEventArgs e)
        {

        }

        private async void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            FindName("HoverArea");

            ShadowPanel.DropShadow.StartAnimation("Opacity",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.9f, TimeSpan.FromMilliseconds(200), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 45.0f, TimeSpan.FromMilliseconds(200), null));

            var colorAnimation = ShadowPanel.DropShadow.Compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = TimeSpan.FromMilliseconds(200);

            colorAnimation.InsertKeyFrame(0.0f, Colors.Black);
            colorAnimation.InsertKeyFrame(1.0f, _hoverColor);

            ShadowPanel.DropShadow.StartAnimation("Color", colorAnimation);

            await HoverArea.Fade(1, 200).StartAsync();
        }

        private async void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Opacity",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.4f, TimeSpan.FromMilliseconds(200), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 25.0f, TimeSpan.FromMilliseconds(200), null));

            var colorAnimation = ShadowPanel.DropShadow.Compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = TimeSpan.FromMilliseconds(200);

            colorAnimation.InsertKeyFrame(0.0f, _hoverColor);
            colorAnimation.InsertKeyFrame(1.0f, Colors.Black);

            ShadowPanel.DropShadow.StartAnimation("Color", colorAnimation);

            if (HoverArea != null)
                await HoverArea.Fade(0, 200).StartAsync();

            UnloadObject(HoverArea);
        }

        private async void ImageExBase_OnImageExOpened(object sender, ImageExOpenedEventArgs e)
        {
            if (_hoverColor != Colors.Black)
                return;

            _hoverColor = await Helpers.ColorHelper.GetDominantHue(new Uri(Track.ArtworkUrl));
        }
    }
}
