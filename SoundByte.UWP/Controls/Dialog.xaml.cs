using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.Controls
{
    public partial class Dialog : UserControl
    {
        public static readonly DependencyProperty DialogContentProperty =
            DependencyProperty.Register(nameof(DialogContent), typeof(object), typeof(Dialog), new PropertyMetadata(null));

        public object DialogContent
        {
            get => GetValue(DialogContentProperty);
            set => SetValue(DialogContentProperty, value);
        }

        public Dialog()
        {
            this.DataContext = this;
        }

        /// <summary>
        /// Show the Dialog on the screen
        /// </summary>
        public void Show()
        {
            // Display the popup
            Opacity = 0.0;
            Visibility = Visibility.Visible;
            // Create the overlay element on the go
            var overlayGrid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Name = "OverlayGrid",
                Background = new SolidColorBrush(new Windows.UI.Color { R = 0, G = 0, B = 0, A = 100 }),
                Opacity = 0.0
            };

            // Set the tap event handler
            overlayGrid.Tapped += async (s, e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Hide);
            };

            // Set the grid z index so it appears 
            // above everything
            Canvas.SetZIndex(overlayGrid, 900);

            // Make sure that the current elements
            // z-index is set correctly
            Canvas.SetZIndex(this, 1000);

            // Add the overlay grid to the parent
            (App.Shell.Content as Grid)?.Children.Add(overlayGrid);

            // Get a reference to this new grid created
            // in the parent page
            var overlayGridUi = (App.Shell.Content as Grid)?.FindName("OverlayGrid") as Grid;

            // Only perform the next logic if
            // the grid exists.
            if (overlayGridUi == null)
            {
                // If this happens, we have bigger problems
                throw new Exception("Overlay Grid was not created within the popup control");
            }

            // Add a render transform to the object and position the object lower
            // then usual
            RenderTransform = new ScaleTransform
            {
                ScaleX = 0.8,
                ScaleY = 0.8
            };
            RenderTransformOrigin = new Point(0.5, 0.5);

            var animationBoard = new Storyboard();
            var overlayOpacityAnimation = new DoubleAnimation
            {
                To = 1.0,
                From = 0.0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new SineEase()
            };
            // Set the target and target property
            Storyboard.SetTarget(overlayOpacityAnimation, overlayGridUi);
            Storyboard.SetTargetProperty(overlayOpacityAnimation, new PropertyPath("Grid.Opacity").Path);

            var popupScaleXAnimation = new DoubleAnimation
            {
                To = 1.0,
                From = 0.8,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase()
            };
            // Set the target and target property
            Storyboard.SetTarget(popupScaleXAnimation, RenderTransform);
            Storyboard.SetTargetProperty(popupScaleXAnimation, new PropertyPath("ScaleX").Path);

            var popupScaleYAnimation = new DoubleAnimation
            {
                To = 1.0,
                From = 0.8,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase()
            };

            Storyboard.SetTarget(popupScaleYAnimation, RenderTransform);
            Storyboard.SetTargetProperty(popupScaleYAnimation, new PropertyPath("ScaleY").Path);

            // Add the animations to the board
            animationBoard.Children.Add(overlayOpacityAnimation);
            animationBoard.Children.Add(popupScaleXAnimation);
            animationBoard.Children.Add(popupScaleYAnimation);

            overlayGrid.Blur(50, 300).Start();
            this.Fade(1, 300).Start();
            // Play both of the animation
            // at the same time
            animationBoard.Begin();

            // Focus on the popup element if on xbox
            if (DeviceHelper.IsXbox)
                Focus(FocusState.Keyboard);
        }

        /// <summary>
        /// Hide the dialog
        /// </summary>
        public void Hide()
        {
            // Get the parent object and
            // then get the popup overlay,
            // continuing only if the popup
            // overlay exists
            var parentPage = App.Shell.Content as Grid;
            var overlay = parentPage?.FindName("OverlayGrid") as Grid;

            var animationBoard = new Storyboard();
            var overlayOpacityAnimation = new DoubleAnimation
            {
                To = 0.0,
                From = 1.0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new SineEase()
            };
            // Set the target and target property
            Storyboard.SetTarget(overlayOpacityAnimation, overlay);
            Storyboard.SetTargetProperty(overlayOpacityAnimation, new PropertyPath("Grid.Opacity").Path);

            var popupScaleXAnimation = new DoubleAnimation
            {
                To = 0.8,
                From = 1.0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase()
            };
            // Set the target and target property
            Storyboard.SetTarget(popupScaleXAnimation, RenderTransform);
            Storyboard.SetTargetProperty(popupScaleXAnimation, new PropertyPath("ScaleX").Path);

            var popupScaleYAnimation = new DoubleAnimation
            {
                To = 0.8,
                From = 1.0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase()
            };

            Storyboard.SetTarget(popupScaleYAnimation, RenderTransform);
            Storyboard.SetTargetProperty(popupScaleYAnimation, new PropertyPath("ScaleY").Path);

            // Add the animations to the board
            animationBoard.Children.Add(overlayOpacityAnimation);
            animationBoard.Children.Add(popupScaleXAnimation);
            animationBoard.Children.Add(popupScaleYAnimation);

            // Play both of the animation
            // at the same time
            animationBoard.Begin();
            overlay.Blur(0, 300).Start();
            this.Fade(0, 300).Start();

            // Call the delete events when the animation
            // is completed
            animationBoard.Completed += (s, e) =>
            {
                // Wait 300ms for animations to finish
                Task.Delay(300).Wait();
                // Delete the overlay grid and 
                // hide the popup
                Visibility = Visibility.Collapsed;
                parentPage?.Children.Remove(overlay);
            };
        }
    }
}