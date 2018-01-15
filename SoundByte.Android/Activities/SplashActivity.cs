using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;
using SoundByte.Android.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Services;

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
        private async void RunStartupLogic()
        {
            if (!AppKeysHelper.KeysValid)
            {
                // If this fails getting the keys, we have an issue and must close the app
                if (!await AndroidAuthorizationHelpers.OnlineAppInitAsync(true))
                    return;
            }

            // Load the SoundByte V3 API
            var secretList = new List<ServiceInfo>
            {
                new ServiceInfo
                {
                    Service = ServiceType.SoundCloud,
                    ClientIds = AppKeysHelper.SoundCloudPlaybackIds,
                    ClientId = AppKeysHelper.SoundCloudClientId
                },
                new ServiceInfo
                {
                    Service = ServiceType.SoundCloudV2,
                    ClientIds = AppKeysHelper.SoundCloudPlaybackIds,
                    ClientId = AppKeysHelper.SoundCloudClientId
                },
                new ServiceInfo
                {
                    Service = ServiceType.Fanburst,
                    ClientId = AppKeysHelper.FanburstClientId,
                },
                new ServiceInfo
                {
                    Service = ServiceType.YouTube,
                    ClientId = AppKeysHelper.YouTubeClientId,
                },
                new ServiceInfo
                {
                    Service = ServiceType.ITunesPodcast
                },
                new ServiceInfo
                {
                    Service = ServiceType.SoundByte,
                    ClientId = AppKeysHelper.SoundByteClientId
                }
            };

            // Init the V3 service
            SoundByteService.Current.Init(secretList);

            MobileCenter.Start(AppKeysHelper.AppCenterClientId,
                typeof(Analytics), typeof(Crashes), typeof(Distribute));

            // Start the main app activity
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}