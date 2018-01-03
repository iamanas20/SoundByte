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
using System.Threading.Tasks;

namespace SoundByte.Core.Services
{
    /// <summary>
    ///     This class handles global app telemetry to all telemetry services
    ///     connected to this app. 
    /// </summary>
    public interface ITelemetryService
    {
        /// <summary>
        ///     Init the Telemetry Service.
        /// </summary>
        /// <param name="gaKey">Google Analytics Tracker Id</param>
        /// <param name="haKey">HockeyApp Client Id</param>
        /// <param name="mcKey">VS Mobile Center Client Id</param>
        Task InitAsync(string gaKey, string haKey, string mcKey);

        /// <summary>
        ///     Track a page/view hit.
        /// </summary>
        /// <param name="pageName">Page/View name</param>
        void TrackPage(string pageName);

        /// <summary>
        ///     Track an event with paramaters.
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="properties">Paramaters for the event</param>
        void TrackEvent(string eventName, Dictionary<string, string> properties = null);

        /// <summary>
        ///     Track an exception
        /// </summary>
        /// <param name="ex">Exception to track</param>
        /// <param name="isFatal">Is the exception fatal (app crash)</param>
        void TrackException(Exception ex, bool isFatal);
    }
}