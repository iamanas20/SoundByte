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
using System.Linq;
using Windows.UI.Notifications;
using Kochava;
using Microsoft.HockeyApp;
using NotificationsExtensions;
using NotificationsExtensions.Toasts;
using SoundByte.UWP.Assets;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     This class handles global app telemetry to all telemetry services
    ///     connected to this app. (Application Insights, HockeyApp, Google Analytics).
    /// </summary>
    public class TelemetryService
    {
        /// <summary>
        ///     Setup the telemetry providers
        /// </summary>
        private TelemetryService()
        {
            try
            {
                _kochavaTracker = new Tracker("kosoundbyte-uwp-13z8hg");

                // Used for crash reporting
                HockeyClient.Current.Configure(AppKeys.HockeyAppClientId);

                LoggingService.Log(LoggingService.LogType.Debug, "Now Processing Telemetry");
            }
            catch
            {
                // ignored
            }
        }

        private Tracker _kochavaTracker;

        public void TrackPage(string pageName)
        {
            try
            {
                var pageNavigationEvent = new EventParameters(EventType.View);
                pageNavigationEvent.SetName(pageName);

                _kochavaTracker.SendEvent(pageNavigationEvent);



                _kochavaTracker.SendEvent("PageNavigation", pageName);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        public void TrackEvent(string eventName, Dictionary<string, string> properties = null)
        {
            try
            {
                _kochavaTracker.SendEvent("Test", eventName);

                // Send a hit to Google Analytics
        //        GoogleAnalyticsClient.Send(HitBuilder.CreateCustomEvent("App", "Action", eventName).Build());
            }
            catch
            {
                // ignored
            }

            LoggingService.Log(LoggingService.LogType.Debug, properties != null
                ? $"{eventName}\n{string.Join(Environment.NewLine, properties.Select(kvp => kvp.Key + ": " + kvp.Value.ToString()))}, "
                : $"{eventName}");
        }

        public void TrackException(Exception exception, bool isFatal)
        {
            try
            {
                HockeyClient.Current.TrackException(exception, new Dictionary<string, string>
                {
                    { "IsFatal", isFatal.ToString() }
                });                
            }
            catch
            {
                // ignored
            }
        }

        private void PopDebugToast(string message)
        {
            if (!SettingsService.Instance.IsDebugModeEnabled)
                return;

            try
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
                                    Text = "SoundByte Debugging"
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
                var toast = new ToastNotification(toastContent.GetXml()) {ExpirationTime = DateTime.Now.AddMinutes(30)};
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            catch
            {
                // Notification platform may not exist, it does not really matter if this is not called
            }  
        }

        #region Service Setup
        private static readonly Lazy<TelemetryService> InstanceHolder =
            new Lazy<TelemetryService>(() => new TelemetryService());

        public static TelemetryService Instance => InstanceHolder.Value;
        #endregion
    }
}