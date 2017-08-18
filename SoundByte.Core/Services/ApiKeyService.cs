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
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;

namespace SoundByte.Core.Services
{
    /// <summary>
    ///     This class contains any keys used by the app. For example
    ///     client IDs and client secrets.
    ///     
    ///     This class exposes two methods of accessing required keys,
    ///     Async and Non-Async. Async is recomended to use 99.99% of the 
    ///     time as it will not block the UI. In cases where async code cannot
    ///     be used, getters are exposed.
    /// 
    /// </summary>
    public static class ApiKeyService
    {
        private class KeyObject
        {
            public string GoogleAnalytics { get; set; }
            public string HockeyAppClientID { get; set; }
            public string AzureMobileCenterClientID { get; set; }
            public string SoundCloudClientID { get; set; }
            public string SoundCloudClientSecret { get; set; }
            public string FanburstClientID { get; set; }
            public string FanbustClientSecret { get; set; }
            public List<string> BackupSoundCloudPlaybackIDs { get; set; }
        }

        private static bool _loaded;

        private static string _googleAnalyticsTrackerId;
        private static string _hockeyAppClientId;
        private static string _azureMobileCenterClientId;
        private static string _soundCloudClientId;
        private static string _soundCloudClientSecret;
        private static string _fanburstClientId;
        private static string _fanburstClientSecret;
        private static List<string> _soundCloudPlaybackClientIds;

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            if (_loaded)
                return;

            AsyncHelper.RunSync(async () =>
            {
                await InitAsync();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task InitAsync()
        {
            _loaded = false;

            await Task.Run(async () =>
            {
                var dataFile = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\app_keys.json");
                var keys = JsonConvert.DeserializeObject<KeyObject>(File.ReadAllText(dataFile.Path));

                _googleAnalyticsTrackerId = keys.GoogleAnalytics;
                _hockeyAppClientId = keys.HockeyAppClientID;
                _azureMobileCenterClientId = keys.AzureMobileCenterClientID;
                _soundCloudClientId = keys.SoundCloudClientID;
                _soundCloudClientSecret = keys.SoundCloudClientSecret;
                _fanburstClientId = keys.FanburstClientID;
                _fanburstClientSecret = keys.FanbustClientSecret;
                _soundCloudPlaybackClientIds = keys.BackupSoundCloudPlaybackIDs;

            }).ContinueWith(task =>
            {
                _loaded = task.Exception == null;
            });
        }

        public static string GoogleAnalyticsTrackerId
        {
            get
            {
                Init();

                return _googleAnalyticsTrackerId;
            }
        }

        public static string HockeyAppClientId
        {
            get
            {
                Init();

                return _hockeyAppClientId;
            }
        }

        public static string AzureMobileCenterClientId
        {
            get
            {
                Init();

                return _azureMobileCenterClientId;
            }
        }

        public static string SoundCloudClientId
        {
            get
            {
                Init();

                return _soundCloudClientId;
            }
        }

        public static string SoundCloudClientSecret
        {
            get
            {
                Init();

                return _soundCloudClientSecret;
            }
        }

        public static string FanburstClientId
        {
            get
            {
                Init();

                return _fanburstClientId;
            }
        }

        public static string FanburstClientSecret
        {
            get
            {
                Init();

                return _fanburstClientSecret;
            }
        }

        public static List<string> SoundCloudPlaybackClientIds
        {
            get
            {
                Init();

                return _soundCloudPlaybackClientIds;
            }
        }
    }
}