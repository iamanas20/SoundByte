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
using Newtonsoft.Json;
using SoundByte.API.Endpoints;

namespace SoundByte.API.Holders
{
    /// <summary>
    ///     Holds all the stream tracks
    /// </summary>
    [JsonObject]
    public class StreamTrackHolder
    {
        /// <summary>
        ///     List of stream items
        /// </summary>
        [JsonProperty("collection")]
        public List<StreamItem> Items { get; set; }

        /// <summary>
        ///     Next items in the list
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}