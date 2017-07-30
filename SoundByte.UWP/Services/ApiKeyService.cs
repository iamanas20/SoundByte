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

using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace SoundByte.UWP.Services
{
    /// <summary>
    /// This class contains any keys used by the app. For example
    /// client IDs and client secrets.
    /// </summary>
    public static class ApiKeyService
    {
        public static string GoogleAnalyticsTrackerId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.GA") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.GA"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("ga");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.GA", liveKey);

                return liveKey;
            }
        } 

        public static string HockeyAppClientId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.HAC") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.HAC"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("hac");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.HAC", liveKey);

                return liveKey;
            }
        }

        public static string AzureMobileCenterClientId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.AMCC") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.AMCC"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("amcc");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.AMCC", liveKey);

                return liveKey;
            }
        }

        public static string SoundCloudClientId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.SCC") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.SCC"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("scc");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.SCC", liveKey);

                return liveKey;
            }
        } 

        public static string SoundCloudClientSecret
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.SCS") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.SCS"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("scs");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.SCS", liveKey);

                return liveKey;
            }
        } 

        public static string FanburstClientId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.FBC") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.FBC"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("fbc");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.FBC", liveKey);

                return liveKey;
            }
        }

        public static string FanburstClientSecret
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.FBS") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.FBS"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("fbs");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.FBS", liveKey);

                return liveKey;
            }
        }

        public static List<string> SoundCloudPlaybackClientIds
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.SCPI") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.SCPI"] as string : null;

                if (key != null)
                    return key.Split(',').ToList();

                var liveKey = SoundByteService.Current.GetSoundBytePlaybackKeys();
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.SCPI", string.Join(",", liveKey));

                return liveKey;
            }
        }
    }
}
