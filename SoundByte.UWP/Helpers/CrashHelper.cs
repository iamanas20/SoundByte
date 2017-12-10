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
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Helpers
{
    public static class CrashHelper
    {
        public static void HandleAppCrashes(Application currentApplication)
        {
            // Log when the app crashes
            CoreApplication.UnhandledErrorDetected += async (sender, args) =>
            {
                try
                {
                    args.UnhandledError.Propagate();
                }
                catch (Exception e)
                {
                    await HandleAppCrashAsync(e);
                }
            };

            // Log when the app crashes
            currentApplication.UnhandledException += async (sender, args) =>
            {
                // We have handled this exception
                args.Handled = true;                
                // Show the exception UI
                await HandleAppCrashAsync(args.Exception);
            };

            TaskScheduler.UnobservedTaskException += async (sender, args) =>
            {
                args.SetObserved();

                await HandleAppCrashAsync(args.Exception);
            };

            LoggingService.Log(LoggingService.LogType.Debug, "Now Handling App Crashes");
        }


        private static async Task HandleAppCrashAsync(Exception ex)
        {
            App.Telemetry.TrackException(ex, true);

            try
            {
                if (!DeviceHelper.IsBackground)
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(
                        async () =>
                        {
                            await NavigationService.Current.CallDialogAsync<CrashDialog>(ex);
                        });
                }      
            }
            catch
            {
                // Do nothing
            }
        }
    }
}