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
using GoogleAnalytics;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Push;
using Microsoft.HockeyApp;
using SoundByte.Core.Helpers;

namespace SoundByte.Core.Services
{
    /// <summary>
    /// This class handles global app telemetry to all telemetry services
    /// connected to this app. (Application Insights, HockeyApp, Google Analytics).
    /// </summary>
    public class TelemetryService
    {
        private Tracker GoogleAnalyticsClient { get; }

        #region Service Setup
        private static TelemetryService _instance;
        public static TelemetryService Current => _instance ?? (_instance = new TelemetryService());
        #endregion

        /// <summary>
        /// Setup the telemetry providers
        /// </summary>
        private TelemetryService()
        {
            try
            {
                // Setup Google Analytics
                AnalyticsManager.Current.DispatchPeriod = TimeSpan.Zero; // Immediate mode, sends hits immediately
                AnalyticsManager.Current.AutoAppLifetimeMonitoring = true; // Handle suspend/resume and empty hit batched hits on suspend
                AnalyticsManager.Current.AppOptOut = false;
                AnalyticsManager.Current.IsEnabled = true;
                AnalyticsManager.Current.IsDebug = false;
                GoogleAnalyticsClient = AnalyticsManager.Current.CreateTracker(ApiKeyService.GoogleAnalyticsTrackerId);

                // Azure Mobile Aalytics and push support
                MobileCenter.Start(ApiKeyService.AzureMobileCenterClientId, typeof(Analytics), typeof(Push));

                // Used for crash reporting
                HockeyClient.Current.Configure(ApiKeyService.HockeyAppClientId);  

#if DEBUG
                // Disable this on debug
                AnalyticsManager.Current.AppOptOut = true;
                AnalyticsManager.Current.IsDebug = true;
                AsyncHelper.RunSync(async () => { await MobileCenter.SetEnabledAsync(false); });
#endif
            }
            catch
            {
                // ignored
            }  
        }
    
        public void TrackPage(string pageName)
        {
            try
            {
                GoogleAnalyticsClient.ScreenName = pageName;
                GoogleAnalyticsClient.Send(HitBuilder.CreateScreenView().Build());
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        public void TrackEvent(string eventName, Dictionary<string, string> properties = null)
        {
            try
            {
                // Send a hit to Google Analytics
                GoogleAnalyticsClient.Send(HitBuilder.CreateCustomEvent("App", "Action", eventName).Build());

                // Send a hit to azure
                Analytics.TrackEvent(eventName, properties);
            }
            catch
            {
                // ignored
            }

            System.Diagnostics.Debug.WriteLine(properties != null
                ? $"[{eventName}]:\n{string.Join(Environment.NewLine, properties.Select(kvp => kvp.Key + ": " + kvp.Value.ToString()))}\n"
                : $"[{eventName}]\n");
        }

        public void TrackException(Exception exception)
        {
            try
            {
                HockeyClient.Current.TrackException(exception);
            }
            catch
            {
                // ignored
            }
        }
    }
}
