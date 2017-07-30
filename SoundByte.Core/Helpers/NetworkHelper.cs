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
using Windows.Networking.Connectivity;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    /// Helpers methods for connecting to the internet
    /// </summary>
    public static class NetworkHelper
    {
        /// <summary>
        ///     Does the application currently have access to the internet.
        /// </summary>
        public static bool HasInternet
        {
            get
            {
                try
                {
                    var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                    return connectionProfile != null &&
                           connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
