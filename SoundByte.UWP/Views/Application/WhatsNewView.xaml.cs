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
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views.Application
{
    /// <summary>
    ///     Open a webview with the current changelog
    /// </summary>
    public sealed partial class WhatsNewView
    {
        public WhatsNewView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            App.Telemetry.TrackPage("What's New View");

            CurrentVersion.Text = $"Current Version: {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";

            await App.SetLoadingAsync(true);

            try
            {
                // Get the changelog string from the azure api
                using (var httpClient = new HttpClient())
                {
                    var changelog =
                        await httpClient.GetStringAsync(
                            new Uri("https://soundbyte.gridentertainment.net/api/v1/app/changelog"));

                    ChangelogView.Text = changelog;
                }
            }
            catch (Exception)
            {
                ChangelogView.Text = "*Error:* An error occured while getting the changelog.";
            }

            await App.SetLoadingAsync(false);
        }
    }
}