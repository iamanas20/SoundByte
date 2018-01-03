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
using Newtonsoft.Json;

namespace SoundByte.Core.Items.SoundByte
{
    /// <summary>
    /// The keys that the SoundByte service returns.
    /// </summary>
    [JsonObject]
    public class AppInitializationKeys
    {
        public string SoundCloudClientId { get; set; }
        public List<string> SoundCloudPlaybackIds { get; set; }
        public string FanburstClientId { get; set; }
        public string YouTubeClientId { get; set; }
        public string YouTubeLoginClientId { get; set; }
        public string LastFmClientId { get; set; }
        public string HockeyAppClientId { get; set; }
        public string GoogleAnalyticsTrackerId { get; set; }
        public string AppCenterClientId { get; set; }
    }
}
