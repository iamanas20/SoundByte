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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleAnalytics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.HockeyApp;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     This class handles global app telemetry to all telemetry services
    ///     connected to this app. (Application Insights, HockeyApp, Google Analytics).
    /// </summary>
    public class TelemetryService
    {
        private bool _isInit;
        private Tracker _googleAnalyticsClient;

        /// <summary>
        /// Setup the telementry service to start handling app telemetry.
        /// </summary>
        /// <param name="googleAnalyticsTrackerId">Google Analytics Tracker ID</param>
        /// <param name="hockeyAppClientId">Hockeyapp Client ID</param>
        /// <param name="mobileCenterClientId">VS Mobile Center Client ID</param>
        /// <returns></returns>
        public async Task InitAsync(string googleAnalyticsTrackerId, string hockeyAppClientId, string mobileCenterClientId)
        {
            // Used for crash reporting
            HockeyClient.Current.Configure(hockeyAppClientId);

            // Used for general analytics
            _googleAnalyticsClient = AnalyticsManager.Current.CreateTracker(googleAnalyticsTrackerId);

            // Used for general analytics, push notifications and crashes
            AppCenter.Start(mobileCenterClientId, typeof(Analytics), typeof(Crashes));
#if DEBUG
            // Disable this on debug
            AnalyticsManager.Current.IsDebug = true;
            AnalyticsManager.Current.AppOptOut = true;
            await Analytics.SetEnabledAsync(false);
#endif
            _isInit = true;

            LoggingService.Log(LoggingService.LogType.Debug, "Now Processing Telemetry");
        }

        public void TrackPage(string pageName)
        {
            // Ignore telementry before service is init
            if (!_isInit)
                return;

            try
            {
                TrackEvent("Page Navigation", new Dictionary<string, string> { {"PageName", pageName} });

                _googleAnalyticsClient.ScreenName = pageName;
                _googleAnalyticsClient.Send(HitBuilder.CreateScreenView().Build());
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
            // Ignore telementry before service is init
            if (!_isInit)
                return;

            try
            {
                Analytics.TrackEvent(eventName, properties);
                _googleAnalyticsClient.Send(HitBuilder.CreateCustomEvent("App", "Action", eventName).Build());
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
            // Ignore crashes before service is init
            if (!_isInit)
                return;

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
    }
}