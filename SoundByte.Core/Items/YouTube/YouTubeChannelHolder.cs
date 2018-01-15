using System.Collections.Generic;
using Newtonsoft.Json;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Items.YouTube
{
    public class YouTubeChannelHolder
    {
        /// <summary>
        ///     Collection of channels
        /// </summary>
        [JsonProperty("items")]
        public List<YouTubeUser> Channels { get; set; }

        /// <summary>
        ///     The next list of items
        /// </summary>
        [JsonProperty("nextPageToken")]
        public string NextList { get; set; }
    }
}
