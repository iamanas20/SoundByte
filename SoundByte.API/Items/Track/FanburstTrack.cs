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
using SoundByte.API.Endpoints;

namespace SoundByte.API.Items.Track
{
    [JsonObject]
    public class FanburstTrack : ITrack
    {
        [JsonObject]
        public class FanburstImages
        {
            [JsonProperty("square_150")]
            public string Square150 { get; set; }

            [JsonProperty("square_250")]
            public string Square250 { get; set; }

            [JsonProperty("square_500")]
            public string Square500 { get; set; }
        }

        [JsonObject]
        public class FanburstUser
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("permalink")]
            public string Permalink { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("avatar_url")]
            public string AvatarUrl { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("published_at")]
        public string PublishedAt { get; set; }

        [JsonProperty("downloadable")]
        public bool IsDownloadable { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("images")]
        public FanburstImages Images { get; set; }

        [JsonProperty("stream_url")]
        public string StreamUrl { get; set; }

        [JsonProperty("user")]
        public FanburstUser User { get; set; }

        /// <summary>
        /// Convert this Fanburst specific track to a universal track.
        /// </summary>
        /// <returns></returns>
        public BaseTrack ToBaseTrack()
        {
            return new BaseTrack
            {
                ServiceType = ServiceType.Fanburst,
                Id = Id,
                Kind = "track", 
                Link = Permalink,
                AudioStreamUrl = string.Empty,
                VideoStreamUrl = string.Empty,
                ArtworkUrl = Images.Square500,
                Title = Title,
                Description = string.Empty,
                Duration = TimeSpan.FromSeconds(Duration),
                Created = DateTime.Parse(PublishedAt),
                LikeCount = 0,
                DislikeCount = 0,
                ViewCount = 0,
                CommentCount = 0,
                Genre = "Unkown"
            };
        }
    }
}
