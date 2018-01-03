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

using Newtonsoft.Json;

namespace SoundByte.Core.Items.YouTube
{
    [JsonObject]

    public class YouTubeThumbnails
    {
        [JsonProperty("default")]
        public YouTubeThumbnailSize DefaultSize { get; set; }

        [JsonProperty("medium")]
        public YouTubeThumbnailSize MediumSize { get; set; }

        [JsonProperty("high")]
        public YouTubeThumbnailSize HighSize { get; set; }
    }
}
