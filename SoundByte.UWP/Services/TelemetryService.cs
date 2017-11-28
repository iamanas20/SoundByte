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
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.HockeyApp;
using SoundByte.Core.Helpers;
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

                // Used for crash reporting
                HockeyClient.Current.Configure(AppKeys.HockeyAppClientId);

                // Used for general analytics
          //      AppCenter.Start(AppKeys.AzureMobileCenterClientId, typeof(Analytics)); // Takes too long to start? Bug in app center.
                _googleAnalyticsClient = AnalyticsManager.Current.CreateTracker(AppKeys.GoogleAnalyticsTrackerId);

                // Log that we have started processing telemetry
                LoggingService.Log(LoggingService.LogType.Debug, "Now Processing Telemetry");

#if DEBUG
                // Disable this on debug
                AnalyticsManager.Current.IsDebug = true;
           //     AsyncHelper.RunSync(async () => { await AppCenter.SetEnabledAsync(false); });
#endif
            }
            catch
            {
                // ignored
            }
        }

        private readonly Tracker _googleAnalyticsClient;


        public void TrackPage(string pageName)
        {
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
            try
            {
       //         Analytics.TrackEvent(eventName, properties);
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

        #region Service Setup
        private static readonly Lazy<TelemetryService> InstanceHolder =
            new Lazy<TelemetryService>(() => new TelemetryService());

        public static TelemetryService Instance => InstanceHolder.Value;
        #endregion
    }
}