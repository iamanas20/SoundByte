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
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using SoundByte.Core;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using ColorThiefDotNet;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI;
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
                if (ItemType == ItemType.Track && Track != null && TrackNowPlaying != null)
                    TrackNowPlaying.Visibility = newTrack?.Id == Track?.Id ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void SoundByteItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            DesktopUserItem.Visibility = Visibility.Collapsed;
            DesktopTrackItem.Visibility = Visibility.Collapsed;
            DesktopUserItem.Visibility = Visibility.Collapsed;

            switch (ItemType)
            {
                case ItemType.Playlist:
                    // Generate and show the desktop playlist item
                    DesktopPlaylistItem.Visibility = Visibility.Visible;
                    break;
                case ItemType.Track:
                    // Generate and show the desktop track item
                    DesktopTrackItem.Visibility = Visibility.Visible;
                    // Update the visibilty
                    TrackNowPlaying.Visibility = PlaybackService.Instance.CurrentTrack?.Id == Track?.Id ? Visibility.Visible : Visibility.Collapsed;                   
                    break;
                case ItemType.User:
                    // Generate and show the desktop user item
                    DesktopUserItem.Visibility = Visibility.Visible;
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

        public static async Task<Windows.UI.Color> GetDominantHue(Uri imageLocation)
        {
            var tcs = new TaskCompletionSource<Windows.UI.Color>();

            // ensure we've precached the image in Storage, not memory. Not sure what
            // will happen if the image is already there from a binding, which it should be.
            await ImageCache.Instance.PreCacheAsync(imageLocation);

            // see if the file is in the cache
            var imgFile = await ImageCache.Instance.GetFileFromCacheAsync(imageLocation);

            if (imgFile != null)
            {
                var decoder = await Task.Run(async () =>
                {
                    var stream = await imgFile.OpenStreamForReadAsync();
                    return await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
                });

                var colorThief = new ColorThief();

                var color = await Task.Run(async () => await colorThief.GetColor(decoder, 5));

                tcs.SetResult(new Windows.UI.Color
                {
                    R = color.Color.R,
                    G = color.Color.G,
                    B = color.Color.B,
                    A = 255
                });
            }
            else
            {
                tcs.SetResult(Colors.Black);
            }

            return tcs.Task.Result;

        }

        public static async Task ColorShadow(UIElement shadowElement, Uri imageUri)
        {
            try
            {
                var panel = (DropShadowPanel)shadowElement;







                // Download and convert to stream in background
                var random = await Task.Run(() => RandomAccessStreamReference.CreateFromUri(imageUri));

                using (var stream = await random.OpenReadAsync())
                {
                    //Create a decoder for the image
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var colorThief = new ColorThief();
                    var color = await colorThief.GetColor(decoder);

                    var colorAnimation = panel.DropShadow.Compositor.CreateColorKeyFrameAnimation();
                    colorAnimation.Duration = TimeSpan.FromMilliseconds(400);

                    colorAnimation.InsertKeyFrame(0.0f, Colors.Black);
                    colorAnimation.InsertKeyFrame(1.0f, new Windows.UI.Color
                    {
                        R = color.Color.R,
                        G = color.Color.G,
                        B = color.Color.B,
                        A = color.Color.A
                    });

                    // Start the animation
                    panel.DropShadow.StartAnimation("Color", colorAnimation);
                }
            }
            catch
            {
                // Not citical, fail in background
            }
        }

        private void DesktopTrackItem_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            StartAnimations(ShadowPanel, Colors.Black);
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
            StartAnimations(PlaylistDropShadow, Colors.Black);
        }

        private void DesktopUserItem_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            StartAnimations(UserDropShadow, Colors.Black);
        }

        private void DesktopUserItem_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            StopAnimation(UserDropShadow);
        }

        private void TrackImage_OnImageOpened(object sender, RoutedEventArgs e)
        {
            //await ColorShadow(ShadowPanel, new Uri(Track.ArtworkUrl));
        }
    }
}