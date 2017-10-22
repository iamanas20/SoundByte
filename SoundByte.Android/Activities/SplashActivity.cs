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

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Services;
using Xamarin.Android.Net;

namespace SoundByte.Android.Activities
{
    [Activity(Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();
            var startupWork = new Task(RunStartupLogic);
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        private void RunStartupLogic()
        {
            // Load the SoundByte V3 API
            var secretList = new List<ServiceSecret>
            {
                new ServiceSecret
                {
                    Service = ServiceType.SoundCloud,
                    ClientId = AppKeys.SoundCloudClientId,
                    ClientSecret = AppKeys.SoundCloudClientSecret,
                },
                new ServiceSecret
                {
                    Service = ServiceType.SoundCloudV2,
                    ClientId = AppKeys.SoundCloudClientId,
                    ClientSecret = AppKeys.SoundCloudClientSecret,
                },
                new ServiceSecret
                {
                    Service = ServiceType.Fanburst,
                    ClientId = AppKeys.FanburstClientId,
                    ClientSecret = AppKeys.FanburstClientSecret,
                },
                new ServiceSecret
                {
                    Service = ServiceType.YouTube,
                    ClientId = AppKeys.YouTubeClientId,
                },
                new ServiceSecret
                {
                    Service = ServiceType.ITunesPodcast
                }
            };

            // Init the V3 service
            SoundByteV3Service.Current.Init(secretList);

            // Better android HTTP support
            SoundByteV3Service.Current.ServiceClientHandler = new AndroidClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            };

            MobileCenter.Start(AppKeys.AzureMobileCenterClientId,
                typeof(Analytics), typeof(Crashes), typeof(Distribute));

            // Start the main app activity
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}