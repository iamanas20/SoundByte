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
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.Web.Http;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Dialogs
{
    /// <summary>
    ///     A dialog to notify the user that the app has crashed.
    /// </summary>
    public sealed partial class CrashDialog
    {
        public CrashDialog() : this(new Exception("This is not a real error message"))
        { }

        public CrashDialog(Exception ex) 
        {
            InitializeComponent();

            ContinueButton.Focus(FocusState.Programmatic);
            MoreInfo.Text = ex.Message;
        }

        private async Task Send()
        {
            if (!NetworkHelper.HasInternet)
                return;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    Description.Document.GetText(TextGetOptions.None, out string description);

                    if (string.IsNullOrEmpty(Contact.Text))
                        Contact.Text = "default@gridentertainment.net";

                    if (string.IsNullOrEmpty(description))
                        description = "n/a";

                    var param = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Title", MoreInfo.Text),
                        new KeyValuePair<string, string>("Description", description),
                        new KeyValuePair<string, string>("Category", "AutoGenerate"),
                        new KeyValuePair<string, string>("ContactEmail", Contact.Text)
                    };

                    var request = await httpClient.PostAsync(new Uri("https://gridentertainment.net/Tickets/Create"),
                        new HttpFormUrlEncodedContent(param));
                    request.EnsureSuccessStatusCode();
                }
            }
            catch
            {
                // ignored
            }
        }

        private async void SendAndCloseApp(object sender, RoutedEventArgs e)
        {
            TelemetryService.Instance.TrackEvent("Crash Dialog - Send and Close App");
            await Send();
            Application.Current.Exit();
        }

        private async void SendAndContinue(object sender, RoutedEventArgs e)
        {
            TelemetryService.Instance.TrackEvent("Crash Dialog - Send and Continue");
            Hide();
            await Send();
        }
    }
}