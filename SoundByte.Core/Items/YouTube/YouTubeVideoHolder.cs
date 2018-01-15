using System.Collections.Generic;
using Newtonsoft.Json;
using SoundByte.Core.Items.Track;

namespace SoundByte.Core.Items.YouTube
{
    [JsonObject]
    public class YouTubeVideoHolder
    {
        /// <summary>
        ///     Collection of tracks
        /// </summary>
        [JsonProperty("items")]
        public List<YouTubeTrack> Tracks { get; set; }

        /// <summary>
        ///     The next list of items
        /// </summary>
        [JsonProperty("nextPageToken")]
        public string NextList { get; set; }
    }
}
