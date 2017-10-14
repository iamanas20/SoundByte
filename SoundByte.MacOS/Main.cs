using System.Collections.Generic;
using AppKit;
using Newtonsoft.Json;
using SoundByte.Core.Items;

namespace SoundByte.MacOS
{
    static class MainClass
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

            public string YouTubeClientID { get; set; }

            public List<string> BackupSoundCloudPlaybackIDs { get; set; }
        }

        static void Main(string[] args)
        {
            // Load the secret file and get the content
            var secretKeyFile = System.IO.File.ReadAllText("app_keys.json");
            var secretKeys = JsonConvert.DeserializeObject<KeyObject>(secretKeyFile);

            // Setup the V3 SoundByte service for SoundCloud
            SoundByte.Core.Services.SoundByteV3Service.Current.Init(new List<ServiceSecret>{
                new ServiceSecret
                {
                    Service = Core.ServiceType.SoundCloud,
                    ClientId = secretKeys.SoundCloudClientID,
                    ClientSecret = secretKeys.SoundCloudClientSecret
                }
            });

            // Normal Apple setup
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
