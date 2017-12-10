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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core;
using SoundByte.Core.Services;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    ///     This page is used to login the user to SoundCloud so we can access their stream etc.
    /// </summary>
    public sealed partial class AccountManagerView
    {
        public AccountManagerView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.Telemetry.TrackPage("Manage Accounts View");

            MainView.Visibility = Visibility.Visible;
            ConnectAccountView.Visibility = Visibility.Collapsed;

            if (DeviceHelper.IsXbox)
            {
                XboxConnectPanel.Visibility = Visibility.Collapsed;
                XboxConnectPanelHost.Visibility = Visibility.Visible;
            }

            RefreshUi();
        }

        private void RefreshUi()
        {
            if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud))
            {
                SoundCloudDisconnectAccount.Visibility = Visibility.Visible;
                SoundCloudViewProfile.Visibility = Visibility.Visible;
                SoundCloudConnectAccount.Visibility = Visibility.Collapsed;
            }
            else
            {
                SoundCloudDisconnectAccount.Visibility = Visibility.Collapsed;
                SoundCloudViewProfile.Visibility = Visibility.Collapsed;
                SoundCloudConnectAccount.Visibility = Visibility.Visible;
            }

            if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.Fanburst))
            {
                FanburstDisconnectAccount.Visibility = Visibility.Visible;
                FanburstViewProfile.Visibility = Visibility.Visible;
                FanburstConnectAccount.Visibility = Visibility.Collapsed;
            }
            else
            {
                FanburstDisconnectAccount.Visibility = Visibility.Collapsed;
                FanburstViewProfile.Visibility = Visibility.Collapsed;
                FanburstConnectAccount.Visibility = Visibility.Visible;
            }

            if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.YouTube))
            {
                YouTubeDisconnectAccount.Visibility = Visibility.Visible;
                YouTubeViewProfile.Visibility = Visibility.Visible;
                YouTubeConnectAccount.Visibility = Visibility.Collapsed;
            }
            else
            {
                YouTubeDisconnectAccount.Visibility = Visibility.Collapsed;
                YouTubeViewProfile.Visibility = Visibility.Collapsed;
                YouTubeConnectAccount.Visibility = Visibility.Visible;
            }   
        }

        private void ConnectXboxOne(object sender, RoutedEventArgs e)
        {
            MainView.Visibility = Visibility.Collapsed;
            ConnectAccountView.Visibility = Visibility.Visible;
            LoginCodeTextBox.Text = string.Empty;
        }

        private async void XboxOneConnectRequest(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<LoginDialog>(ServiceType.SoundCloud, true, LoginCodeTextBox.Text);
        }

        private void NavigateToXboxConnect(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(XboxAccountView));
        }

        private void XboxConnectGoBack(object sender, RoutedEventArgs e)
        {
            MainView.Visibility = Visibility.Visible;
            ConnectAccountView.Visibility = Visibility.Collapsed;
        }


        #region Navigate Profile Methods
        private void NavigateSoundCloudProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteV3Service.Current.GetConnectedUser(ServiceType.SoundCloud));
        }

        private void NavigateFanburstProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteV3Service.Current.GetConnectedUser(ServiceType.Fanburst));
        }

        private void NavigateYouTubeProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteV3Service.Current.GetConnectedUser(ServiceType.YouTube));
        }
        #endregion

        #region Connect Account Methods
        private async void ConnectSoundCloudAccount(object sender, RoutedEventArgs e)
        {
            // Connect Account
            await NavigationService.Current.CallDialogAsync<LoginDialog>(ServiceType.SoundCloud, false, "");
            RefreshUi();
        }

        private async void ConnectFanburstAccount(object sender, RoutedEventArgs e)
        {
            // Connect Account
            await NavigationService.Current.CallDialogAsync<LoginDialog>(ServiceType.Fanburst, false, "");
            RefreshUi();
        }

        private async void ConnectYouTubeAccount(object sender, RoutedEventArgs e)
        {
            // Connect Account
            await NavigationService.Current.CallDialogAsync<LoginDialog>(ServiceType.YouTube, false, "");
            RefreshUi();

        }
        #endregion

        #region Disconnect Account Methods
        private void DisconnectSoundCloudAccount(object sender, RoutedEventArgs e)
        {
            SoundByteV3Service.Current.DisconnectService(ServiceType.SoundCloud);
            RefreshUi();
        }

        private void DisconnectFanburstAccount(object sender, RoutedEventArgs e)
        {
            SoundByteV3Service.Current.DisconnectService(ServiceType.Fanburst);
            RefreshUi();
        }

        private void DisconnectYouTubeAccount(object sender, RoutedEventArgs e)
        {
            SoundByteV3Service.Current.DisconnectService(ServiceType.YouTube);
            RefreshUi();
        }
        #endregion
    }
}