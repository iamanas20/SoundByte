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
using SoundByte.API.Endpoints;

namespace SoundByte.API.Holders
{
    /// <summary>
    ///     Holds a playlist
    /// </summary>
    [JsonObject]
    [Obsolete]
    public class LikePlaylistBootstrap
    {
        /// <summary>
        ///     A playlist
        /// </summary>
        [JsonProperty("playlist")]
        public Playlist Playlist { get; set; }
    }

    /// <summary>
    ///     Holds the users playlists
    /// </summary>
    [JsonObject]
    [Obsolete]
    public class PlaylistHolder
    {
        /// <summary>
        ///     List of sub playlists
        /// </summary>
        [JsonProperty("collection")]
        public List<LikePlaylistBootstrap> Playlists { get; set; }

        /// <summary>
        ///     The next list of items
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }

    /// <summary>
    ///     Holder for searched playlist items
    /// </summary>
    [JsonObject]
    [Obsolete]
    public class SearchPlaylistHolder
    {
        /// <summary>
        ///     List of playlists
        /// </summary>
        [JsonProperty("collection")]
        public List<Playlist> Playlists { get; set; }

        /// <summary>
        ///     The next list of items
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}