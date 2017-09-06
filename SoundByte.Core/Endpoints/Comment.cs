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
using Newtonsoft.Json;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Endpoints
{
    /// <summary>
    ///     This class represents a comment object within the SoundCloud API
    /// </summary>
    [JsonObject]
    [Obsolete]
    public class Comment
    {
        /// <summary>
        ///     Comment body
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        ///     The date and time that this comment was posted.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Object ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     The track that this comment was posted on
        /// </summary>
        [JsonProperty("track")]
        public SoundCloudTrack Track { get; set; }

        /// <summary>
        ///     At what time in the track was this posted
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        ///     The user who posted this comment
        /// </summary>
        [JsonProperty("user")]
        public SoundCloudUser User { get; set; }
    }
}