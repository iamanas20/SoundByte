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

using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    ///     This page handles track playback and connection to
    ///     the background audio task.
    /// </summary>
    public sealed partial class NowPlayingView
    {
        /// <summary>
        ///     Setup page and init the xaml
        /// </summary>
        public NowPlayingView()
        {
            InitializeComponent();
            // Set the data context
            DataContext = ViewModel;
            // Page has been unloaded from UI
            Unloaded += (s, e) => ViewModel.Dispose();

            SizeChanged += (sender, args) =>
            {
                if (IsEnhanced)
                    ShowOverlay();
                else
                    HideOverlay();
            };
        }

        // Main page view model
        public NowPlayingViewModel ViewModel { get; } = new NowPlayingViewModel();

        private bool IsEnhanced { get; set; }

        /// <summary>
        ///     Setup the view model, passing in the navigation events.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Setup view model
            ViewModel.SetupModel();

            // Track Event
            TelemetryService.Instance.TrackPage("Now Playing View");

            if (DeviceHelper.IsDesktop)
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor =
                    new Color {R = 0, G = 0, B = 0, A = 20};
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor =
                    new Color {R = 0, G = 0, B = 0, A = 60};
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = Colors.White;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = Colors.White;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = Colors.White;
            }                

            // Hide the overlay for a new session
            HideOverlay();

            if (!DeviceHelper.IsDesktop)
            {
                CompactViewButton.Visibility = Visibility.Collapsed;
                FullScreenButton.Visibility = Visibility.Collapsed;
                EnhanceButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Track_BackRequested(object sender, BackRequestedEventArgs e)
        {
            HideOverlay();
        }

        /// <summary>
        ///     Clean the view model
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.CleanModel();

            var textColor = Windows.UI.Xaml.Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Colors.White
                : Colors.Black;

            if (DeviceHelper.IsDesktop)
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor =
                    new Color {R = 0, G = 0, B = 0, A = 20};
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor =
                    new Color {R = 0, G = 0, B = 0, A = 60};
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = textColor;

            }

            HideOverlay();
        }

        private void HideOverlay()
        {
            IsEnhanced = false;

            App.OverrideBackEvent = false;
            SystemNavigationManager.GetForCurrentView().BackRequested -= Track_BackRequested;

            ButtonHolder.Visibility = Visibility.Visible;
            ButtonHolder.Offset(0, 0, 450).Fade(1, 250).Start();

            EnhanceButton.Rotate(0, (float) EnhanceButton.ActualWidth / 2, (float) EnhanceButton.ActualHeight / 2, 450)
                .Offset(0, 0, 450).Start();

            var moreInfoAnimation = MoreInfoScreen.Fade(0, 450).Offset(0, (float) RootGrid.ActualHeight, 450);
            moreInfoAnimation.Completed += (o, args) =>
            {
                MoreInfoScreen.Visibility = Visibility.Collapsed;
                MoreInfoPivot.SelectedIndex = 0;
            };
            moreInfoAnimation.Start();

            TrackInfoHolder.Offset(0, 0, 450).Scale(1, 1, 0, 0, 450).Start();

            BlurOverlay.Fade(0, 450).Start();
        }

        private void ShowOverlay()
        {
            TelemetryService.Instance.TrackEvent("Show Now Playing Overlay");

            IsEnhanced = true;

            App.OverrideBackEvent = true;
            SystemNavigationManager.GetForCurrentView().BackRequested += Track_BackRequested;

            var buttonHolderShowAnimation = ButtonHolder.Offset(0, 120, 450).Fade(0, 250);
            buttonHolderShowAnimation.Completed += (o, args) => { ButtonHolder.Visibility = Visibility.Collapsed; };
            buttonHolderShowAnimation.Start();

            EnhanceButton
                .Rotate(180, (float) EnhanceButton.ActualWidth / 2, (float) EnhanceButton.ActualHeight / 2, 450)
                .Offset(0, -1.0f * ((float) RootGrid.ActualHeight - (float) EnhanceButton.ActualHeight - 160), 450)
                .Start();

            MoreInfoScreen.Visibility = Visibility.Visible;
            MoreInfoPivot.SelectedIndex = 0;
            MoreInfoScreen.Fade(1, 450, 150).Offset(0, 0, 450, 150).Start();

            TrackInfoHolder
                .Offset(0, -1.0f * ((float) RootGrid.ActualHeight - (float) TrackInfoHolder.ActualHeight - 40), 450)
                .Scale(0.8f, 0.8f, 0, 0, 450).Start();

            BlurOverlay.Fade(1, 450).Start();
        }

        /// <summary>
        ///     This cannot go in the view model, as we change UI elements A LOT here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTransition(object sender, RoutedEventArgs e)
        {
            if (IsEnhanced)
                HideOverlay();
            else
                ShowOverlay();
        }

        private void VideoOverlay_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            VideoOverlay.Position = PlaybackService.Instance.Player.PlaybackSession.Position;
            VideoOverlay.Fade(1, 450).Start();
        }
    }
}