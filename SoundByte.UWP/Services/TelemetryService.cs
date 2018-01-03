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
using SoundByte.Core.Services;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     This class handles global app telemetry to all telemetry services
    ///     connected to this app. 
    /// </summary>
    public class TelemetryService : ITelemetryService
    {
        private bool _isInit;
        private Tracker _googleAnalyticsClient;

        /// <summary>
        ///     Init the Telemetry Service.
        /// </summary>
        /// <param name="gaKey">Google Analytics Tracker Id</param>
        /// <param name="haKey">HockeyApp Client Id</param>
        /// <param name="mcKey">VS Mobile Center Client Id</param>
        public async Task InitAsync(string gaKey, string haKey, string mcKey)
    {
            // Used for crash reporting
            HockeyClient.Current.Configure(haKey);

            // Used for general analytics
            _googleAnalyticsClient = AnalyticsManager.Current.CreateTracker(gaKey);

            // Used for general analytics, push notifications and crashes
            AppCenter.Start(mcKey, typeof(Analytics), typeof(Crashes));
#if DEBUG
            // Disable this on debug
            AnalyticsManager.Current.IsDebug = true;
            AnalyticsManager.Current.AppOptOut = true;
            await Analytics.SetEnabledAsync(false);
#endif
            _isInit = true;

            LoggingService.Log(LoggingService.LogType.Debug, "Now Processing Telemetry");
        }

        /// <summary>
        ///     Track a page/view hit.
        /// </summary>
        /// <param name="pageName">Page/View name</param>
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
        ///     Track an event with paramaters.
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="properties">Paramaters for the event</param>
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

        /// <summary>
        ///     Track an exception
        /// </summary>
        /// <param name="ex">Exception to track</param>
        /// <param name="isFatal">Is the exception fatal (app crash)</param>
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