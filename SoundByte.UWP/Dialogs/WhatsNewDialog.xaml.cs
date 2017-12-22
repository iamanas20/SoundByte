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
using System.Net.Http;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class WhatsNewDialog 
    {
        public WhatsNewDialog()
        {
            InitializeComponent();
            Opened += WhatsNewDialog_Opened;
        }

        private async void WhatsNewDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            App.Telemetry.TrackPage("What's New Dialog");

            ProgressRing.IsActive = true;

            try
            {
                // Get the changelog string from the azure api
                using (var httpClient = new HttpClient())
                {
                    var changelog =
                        await httpClient.GetStringAsync(
                            new Uri(
                                $"https://soundbytemedia.com/api/v1/app/changelog?platform=uwp&version={Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}"));

                    ChangelogView.Text = changelog;
                }
            }
            catch (Exception)
            {
                ChangelogView.Text = "*Error:* An error occured while getting the changelog.";
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }
    }
}