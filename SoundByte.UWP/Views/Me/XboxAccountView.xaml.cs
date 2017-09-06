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
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Microsoft.AspNet.SignalR.Client;
using SoundByte.API;
using SoundByte.API.Items;
using SoundByte.UWP.Services;


namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    ///     Page used on Xbox to login via PC
    /// </summary>
    public sealed partial class XboxAccountView
    {
        public XboxAccountView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("Xbox Account View");

            var generator = new Random();
            var randomCode = generator.Next(0, 100000).ToString("D5");

            RandomCodeText.Text = randomCode;
            await BackendService.Instance.LoginXboxConnect(randomCode);

            BackendService.Instance.LoginHub.On<LoginToken>("RecieveLoginInfo", async info =>
            {
                

                // Create the password vault
                var vault = new PasswordVault();

                if (info.ServiceType == ServiceType.SoundCloud)
                {
                    // Store the values in the vault
                    vault.Add(new PasswordCredential("SoundByte.SoundCloud", "Token",
                        info.AccessToken));
                    vault.Add(new PasswordCredential("SoundByte.SoundCloud", "Scope",
                        info.Scope));
                }
                else
                {
                    // Store the values in the vault
                    vault.Add(new PasswordCredential("SoundByte.FanBurst", "Token",
                        info.AccessToken));
                }

                TelemetryService.Instance.TrackEvent("Login Successful",
                    new Dictionary<string, string>
                    {
                        {"Service", "Xbox Connect"}
                    });

                await new MessageDialog("Connected!").ShowAsync();

                App.NavigateTo(typeof(HomeView));
            });
        }

  

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await BackendService.Instance.LoginXboxDisconnect(RandomCodeText.Text);
        }
    }
}
