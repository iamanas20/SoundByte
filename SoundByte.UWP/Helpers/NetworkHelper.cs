using System;
using Windows.Networking.Connectivity;

namespace SoundByte.UWP.Helpers
{
    /// <summary>
    ///     Helpers methods for connecting to the internet
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