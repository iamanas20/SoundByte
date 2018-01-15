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
        public class AccountManagerArgs
        {
            public int PivotIndex { get; set; }
        }

        public AccountManagerView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as AccountManagerArgs;

            if (args != null)
            {
                Pivot.SelectedIndex = args.PivotIndex;
            }


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
            if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundByte))
            {
                SoundByteConnectedView.Visibility = Visibility.Visible;
                SoundByteDisconnectedView.Visibility = Visibility.Collapsed;

            }
            else
            {
                SoundByteConnectedView.Visibility = Visibility.Collapsed;
                SoundByteDisconnectedView.Visibility = Visibility.Visible;
            }

            if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud))
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

            if (SoundByteService.Current.IsServiceConnected(ServiceType.Fanburst))
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

            if (SoundByteService.Current.IsServiceConnected(ServiceType.YouTube))
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
            App.NavigateTo(typeof(UserView), SoundByteService.Current.GetConnectedUser(ServiceType.SoundCloud));
        }

        private void NavigateFanburstProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteService.Current.GetConnectedUser(ServiceType.Fanburst));
        }

        private void NavigateYouTubeProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteService.Current.GetConnectedUser(ServiceType.YouTube));
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

        private async void ConnectSoundByteAccount(object sender, RoutedEventArgs e)
        {
            // Connect Account
            await NavigationService.Current.CallDialogAsync<LoginDialog>(ServiceType.SoundByte, false, "");
            RefreshUi();

        }
        #endregion

        #region Disconnect Account Methods
        private void DisconnectSoundCloudAccount(object sender, RoutedEventArgs e)
        {
            SoundByteService.Current.DisconnectService(ServiceType.SoundCloud);
            RefreshUi();
        }

        private void DisconnectSoundByteAccount(object sender, RoutedEventArgs e)
        {
            SoundByteService.Current.DisconnectService(ServiceType.SoundByte);
            RefreshUi();
        }

        private void DisconnectFanburstAccount(object sender, RoutedEventArgs e)
        {
            SoundByteService.Current.DisconnectService(ServiceType.Fanburst);
            RefreshUi();
        }

        private void DisconnectYouTubeAccount(object sender, RoutedEventArgs e)
        {
            SoundByteService.Current.DisconnectService(ServiceType.YouTube);
            RefreshUi();
        }
        #endregion
    }
}