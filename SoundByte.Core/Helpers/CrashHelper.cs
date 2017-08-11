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
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp;
using NotificationsExtensions;
using NotificationsExtensions.Toasts;
using SoundByte.Core.Dialogs;
using SoundByte.Core.Services;

namespace SoundByte.Core.Helpers
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
                    // Show the exception UI
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
        }

        private static void ShowCrashNotification(string message)
        {
            // Generate a notification
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = "SoundByte Crash"
                            },

                            new AdaptiveText
                            {
                                Text = message
                            }
                        }
                    }
                }
            };

            // Show the notification
            var toast = new ToastNotification(toastContent.GetXml()) { ExpirationTime = DateTime.Now.AddMinutes(30) };
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private static async Task HandleAppCrashAsync(Exception ex)
        {
            // Track the error
            TelemetryService.Current.TrackException(ex);

            try
            {
                if (!DeviceHelper.IsBackground)
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(
                        async () => { await new CrashDialog(ex).ShowAsync(); });
                }      
                else
                {
                    ShowCrashNotification(ex.Message);
                }
            }
            catch
            {
                ShowCrashNotification(ex.Message);
            }
        }
    }
}