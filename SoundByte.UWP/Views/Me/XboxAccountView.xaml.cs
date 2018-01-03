/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
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
using Windows.UI.Xaml.Navigation;
using Microsoft.AspNet.SignalR.Client;
using SoundByte.Core.Items;
using SoundByte.Core.Services;
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
            App.Telemetry.TrackPage("Xbox Account View");

            var generator = new Random();
            var randomCode = generator.Next(0, 100000).ToString("D5");

            RandomCodeText.Text = randomCode;
            await BackendService.Instance.LoginXboxConnect(randomCode);

            BackendService.Instance.LoginHub.On<LoginToken>("RecieveLoginInfo", info =>
            {
                // Login with the gen3.0 service
                SoundByteService.Current.ConnectService(info.ServiceType, info);
            });
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await BackendService.Instance.LoginXboxDisconnect(RandomCodeText.Text);
        }
    }
}
