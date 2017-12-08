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
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Microsoft.Toolkit.Uwp.Helpers;
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

            Loaded += async (s, e) =>
            {
                PlaybackService.Instance.Player.CurrentStateChanged += PlayerOnCurrentStateChanged;
                PlaybackService.Instance.OnCurrentTrackChanged += CurrentTrackChanged;

                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    if (PlaybackService.Instance.CurrentTrack?.Id == Track?.Id)
                    {
                        FindName("TrackNowPlaying");
                    }
                    else
                    {
                        UnloadObject(TrackNowPlaying);
                    }
                });
            };

            Unloaded += (s, e) =>
            {
                PlaybackService.Instance.Player.CurrentStateChanged -= PlayerOnCurrentStateChanged;
                PlaybackService.Instance.OnCurrentTrackChanged -= CurrentTrackChanged;
            };
        }

        private async void PlayerOnCurrentStateChanged(MediaPlayer sender, object args)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                if (PlaybackService.Instance.CurrentTrack?.Id == Track?.Id)
                {
                    if (sender.CurrentState == MediaPlayerState.Playing)
                    {
                        FindName("TrackNowPlaying");
                    }
                    else
                    {
                        UnloadObject(TrackNowPlaying);
                    }
                }
            });
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
            FindName("HoverArea");

            ShadowPanel.DropShadow.StartAnimation("Offset.Y",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 10.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("Opacity",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.9f, TimeSpan.FromMilliseconds(200), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 45.0f, TimeSpan.FromMilliseconds(200), null));

            TrackImage.Blur(5, 200).Start();
            await HoverArea.Fade(1, 200).StartAsync();
        }

        private async void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Offset.Y",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 6.0f, TimeSpan.FromMilliseconds(250), null));

            ShadowPanel.DropShadow.StartAnimation("Opacity",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.4f, TimeSpan.FromMilliseconds(200), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 25.0f, TimeSpan.FromMilliseconds(200), null));

            var colorAnimation = ShadowPanel.DropShadow.Compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = TimeSpan.FromMilliseconds(200);

            TrackImage.Blur(0, 200).Start();

            if (HoverArea != null)
                await HoverArea.Fade(0, 200).StartAsync();

            UnloadObject(HoverArea);
        }
    }
}
