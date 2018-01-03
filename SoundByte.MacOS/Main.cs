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
            // ReSharper disable once InconsistentNaming
            public string SoundCloudClientID { get; set; }
        }

        static void Main(string[] args)
        {
            // Load the secret file and get the content
            var secretKeyFile = System.IO.File.ReadAllText("app_keys.json");
            var secretKeys = JsonConvert.DeserializeObject<KeyObject>(secretKeyFile);

            // Setup the V3 SoundByte service for SoundCloud
            Core.Services.SoundByteService.Current.Init(new List<ServiceInfo>{
                new ServiceInfo
                {
                    Service = Core.ServiceType.SoundCloud,
                    ClientId = secretKeys.SoundCloudClientID
                }
            });

            // Normal Apple setup
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
