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

using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    /// Static methods for detecting device
    /// </summary>
    public static class DeviceHelper
    {
        /// <summary>
        /// Is the app running on xbox
        /// </summary>
        public static bool IsXbox => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox";

        /// <summary>
        /// Is the app runnning on a phone
        /// </summary>
        public static bool IsMobile => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";

        /// <summary>
        /// Is the app running on desktop
        /// </summary>
        public static bool IsDesktop => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop";

        /// <summary>
        /// Is the application fullscreen.
        /// </summary>
        public static bool IsDeviceFullScreen => ApplicationView.GetForCurrentView().IsFullScreenMode;

        /// <summary>
        /// Is the app currently in the background.
        /// </summary>
        public static bool IsBackground { get; set; }
    }
}
