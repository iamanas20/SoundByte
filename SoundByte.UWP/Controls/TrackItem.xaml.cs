using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Services;
using SoundByte.UWP.UserControls;
using UICompositionAnimations.Composition;
using WinRTXamlToolkit.Controls.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SoundByte.UWP.Controls
{
    public sealed partial class TrackItem : UserControl
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
                TrackNowPlaying.Visibility = newTrack?.Id == Track?.Id ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void Share(object sender, RoutedEventArgs e)
        {

        }

        private void AddToPlaylist(object sender, RoutedEventArgs e)
        {

        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Opacity",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.9f, TimeSpan.FromMilliseconds(150), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 45.0f, TimeSpan.FromMilliseconds(150), null));

            var colorAnimation = ShadowPanel.DropShadow.Compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = TimeSpan.FromMilliseconds(150);

            colorAnimation.InsertKeyFrame(0.0f, Colors.Black);
            colorAnimation.InsertKeyFrame(1.0f, _hoverColor);

            ShadowPanel.DropShadow.StartAnimation("Color", colorAnimation);

            HoverArea.Fade(1, 150).Start();
            TrackImage.Blur(15, 150).Start();
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            ShadowPanel.DropShadow.StartAnimation("Opacity",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 0.4f, TimeSpan.FromMilliseconds(150), null));

            ShadowPanel.DropShadow.StartAnimation("BlurRadius",
                ShadowPanel.DropShadow.Compositor.CreateScalarKeyFrameAnimation(null, 25.0f, TimeSpan.FromMilliseconds(150), null));

            var colorAnimation = ShadowPanel.DropShadow.Compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = TimeSpan.FromMilliseconds(150);

            colorAnimation.InsertKeyFrame(0.0f, _hoverColor);
            colorAnimation.InsertKeyFrame(1.0f, Colors.Black);

            ShadowPanel.DropShadow.StartAnimation("Color", colorAnimation);

            HoverArea.Fade(0, 150).Start();
            TrackImage.Blur(0, 150).Start();
        }

        private async void ImageExBase_OnImageExOpened(object sender, ImageExOpenedEventArgs e)
        {
            if (_hoverColor != Colors.Black)
                return;

            _hoverColor = await SoundByteItem.GetDominantHue(new Uri(Track.ArtworkUrl));
        }
    }
}
