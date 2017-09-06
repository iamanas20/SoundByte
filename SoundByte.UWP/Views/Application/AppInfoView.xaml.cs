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
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Services.Store.Engagement;
using Newtonsoft.Json;
using SoundByte.API.Items;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;
using SoundByte.UWP.Views.General;

namespace SoundByte.UWP.Views.Application
{
    /// <summary>
    ///     This is the main settings/about page for the app.
    ///     is handled here
    /// </summary>
    public sealed partial class AppInfoView
    {
        // View model for the settings page
        public SettingsViewModel ViewModel = new SettingsViewModel();

        /// <summary>
        ///     Setup the page
        /// </summary>
        public AppInfoView()
        {
            // Initialize XAML Components
            InitializeComponent();
            // Set the datacontext
            DataContext = ViewModel;
            // Page has been unloaded from UI
            Unloaded += (s, e) => ViewModel.Dispose();
        }

        // The settings object, we bind to this to change values
        public SettingsService SettingsService { get; set; } = SettingsService.Instance;

        /// <summary>
        ///     Called when the user navigates to the page
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Track Event
            TelemetryService.Instance.TrackPage("Info View");
            // TEMP, Load the page
            LoadSettingsPage();

            // Set the app version
            AppVersion.Text =
                $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            AppBuildBranch.Text = "...";
            AppBuildTime.Text = "...";

            var dataFile = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\build_info.json");
            var buildData =
                await Task.Run(() => JsonConvert.DeserializeObject<BuildInformation>(File.ReadAllText(dataFile.Path)));

            AppBuildBranch.Text = buildData.BuildBranch;
            AppBuildTime.Text = buildData.BuildTime;
        }

        public async void NavigateBugs()
        {
            await Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/GvC5iXmJSo"));
        }

        public async void NavigateFeedback()
        {
            var launcher = StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        public async void NavigatePrivacy()
        {
            await Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/Y5jGLtoFXs"));
        }

        public async void NavigateReddit()
        {
            await Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/68vfoKLYJS"));
        }

        public async void NavigateFacebook()
        {
            await Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/rOye5hzCXt"));
        }

        public async void NavigateGitHub()
        {
            await Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/O3i37tbVVO"));
        }

        public void NavigateNew()
        {
            App.NavigateTo(typeof(WhatsNewView));
        }

        /// <summary>
        ///     Called when the user taps on the rate_review button
        /// </summary>
        public async void RateAndReview()
        {
            TelemetryService.Instance.TrackPage("Rate and Review App");

            await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        }

        private void LoadSettingsPage()
        {
            ViewModel.IsComboboxBlockingEnabled = true;
            // Get the saved language
            var appLanguage = SettingsService.Instance.CurrentAppLanguage;
            // Check that the string is not empty
            if (!string.IsNullOrEmpty(appLanguage))
                switch (appLanguage)
                {
                    case "en-US":
                        LanguageComboBox.SelectedItem = Language_English_US;
                        break;
                    case "fr":
                        LanguageComboBox.SelectedItem = Language_French_FR;
                        break;
                    case "nl":
                        LanguageComboBox.SelectedItem = Language_Dutch_NL;
                        break;
                    default:
                        LanguageComboBox.SelectedItem = Language_English_US;
                        break;
                }
            else
                LanguageComboBox.SelectedItem = Language_English_US;

            switch (SettingsService.Instance.ApplicationThemeType)
            {
                case AppTheme.Default:
                    themeComboBox.SelectedItem = defaultTheme;
                    break;
                case AppTheme.Light:
                    themeComboBox.SelectedItem = lightTheme;
                    break;
                case AppTheme.Dark:
                    themeComboBox.SelectedItem = darkTheme;
                    break;
                default:
                    themeComboBox.SelectedItem = defaultTheme;
                    break;
            }

            // Enable combo boxes
            ViewModel.IsComboboxBlockingEnabled = false;
        }

        private async void AppThemeComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.IsComboboxBlockingEnabled)
                return;

            switch (((ComboBoxItem) (sender as ComboBox)?.SelectedItem)?.Name)
            {
                case "defaultTheme":
                    SettingsService.Instance.ApplicationThemeType = AppTheme.Default;
                    ((MainShell) Window.Current.Content).RequestedTheme = ElementTheme.Default;
                    break;
                case "darkTheme":
                    SettingsService.Instance.ApplicationThemeType = AppTheme.Dark;
                    ((MainShell) Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                    break;
                case "lightTheme":
                    SettingsService.Instance.ApplicationThemeType = AppTheme.Light;
                    ((MainShell) Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    break;
                default:
                    SettingsService.Instance.ApplicationThemeType = AppTheme.Default;
                    ((MainShell) Window.Current.Content).RequestedTheme = ElementTheme.Default;
                    break;
            }


            var restartDialog = new ContentDialog
            {
                Title = "App Restart",
                Content = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = "The app needs to be restarted in order for the changes to correctly take effect."
                },
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = "Close"
            };

            await restartDialog.ShowAsync();
        }
    }
}