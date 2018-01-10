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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SoundByte.Core.Items.Podcast
{
    [JsonObject]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class PodcastShow 
    {
        [JsonProperty("trackId")]
        public int Id { get; set; }

        [JsonProperty("artistName")]
        public string Username { get; set; }

        [JsonProperty("trackName")]
        public string Title { get; set; }

        [JsonProperty("feedUrl")]
        public string FeedUrl { get; set; }

        [JsonProperty("artworkUrl600")]
        public string ArtworkUrl { get; set; }

        [JsonProperty("trackCount")]
        public int TrackCount { get; set; }

        [JsonProperty("releaseDate")]
        public DateTime Created { get; set; }

        [JsonProperty("genres")]
        public List<string> Genres { get; set; } 
    }
}
