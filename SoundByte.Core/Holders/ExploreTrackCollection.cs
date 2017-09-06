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
using Newtonsoft.Json;
using SoundByte.Core.Items.Track;

namespace SoundByte.Core.Holders
{
    [JsonObject]
    [Obsolete]
    public class ExploreTrackCollection
    {
        /// <summary>
        ///     The track object
        /// </summary>
        [JsonProperty("track")]
        public SoundCloudTrack Track { get; set; }
    }

    /// <summary>
    ///     Used when deserlizing charts. Provided is the list of tracks and
    ///     the uri to the next list.
    /// </summary>
    [JsonObject]
    [Obsolete]
    public class ExploreTrackHolder
    {
        /// <summary>
        ///     The list of items
        /// </summary>
        [JsonProperty("collection")]
        public List<ExploreTrackCollection> Items { get; set; }

        /// <summary>
        ///     Next item in the list
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}