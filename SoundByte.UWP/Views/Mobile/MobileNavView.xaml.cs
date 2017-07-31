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
using SoundByte.Core.Services;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views.Application;
using SoundByte.UWP.Views.Me;

namespace SoundByte.UWP.Views.Mobile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MobileNavView
    {
        public SoundByteService Service { get; } = SoundByteService.Current;

        public MobileNavView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("Mobile Nav Page");

            if (SoundByteService.Current.IsSoundCloudAccountConnected)
            {
                LoginSoundByteButton.Visibility = Visibility.Collapsed;
                ViewUserProfileButton.Visibility = Visibility.Visible;

                UserLikesButton.Visibility = Visibility.Visible;
                UserPlaylistsButton.Visibility = Visibility.Visible;
                UserNotificationsButton.Visibility = Visibility.Visible;
                UserHistoryButton.Visibility = Visibility.Visible;
                UserUploadButton.Visibility = Visibility.Visible;
            }
            else
            {
                LoginSoundByteButton.Visibility = Visibility.Visible;
                ViewUserProfileButton.Visibility = Visibility.Collapsed;

                UserLikesButton.Visibility = Visibility.Collapsed;
                UserPlaylistsButton.Visibility = Visibility.Collapsed;
                UserNotificationsButton.Visibility = Visibility.Collapsed;
                UserHistoryButton.Visibility = Visibility.Collapsed;
                UserUploadButton.Visibility = Visibility.Collapsed;
            }
        }

        private void NavigateUserProfile() => App.NavigateTo(typeof(UserView), SoundByteService.Current.CurrentUser);

        private void NavigateLogin() => App.NavigateTo(typeof(AccountView));

        private void NavigateLikes() => App.NavigateTo(typeof(LikesView));

        private void NavigatePlaylists() => App.NavigateTo(typeof(PlaylistsView));

        private void NavigateNotifications() => App.NavigateTo(typeof(NotificationsView));

        private void NavigateHistory() => App.NavigateTo(typeof(HistoryView));

        private void NavigateUpload() => App.NavigateTo(typeof(UploadView));

        private void NavigateSettings() => App.NavigateTo(typeof(SettingsView));
    }
}
