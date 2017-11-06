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
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoundByte.Core.Items.User;
using System.Threading;

namespace SoundByte.Core.Items.Track
{
    [JsonObject]
    public class FanburstTrack : ITrack
    {
        public FanburstTrack()
        {
        }

        public FanburstTrack(string id)
        {
            Id = id;
        }

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

        public BaseTrack AsBaseTrack => ToBaseTrack();

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
                Link = Url,
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
                Genre = "Unkown",
                User = User.ToBaseUser()
            };
        }

        public async Task<BaseTrack.CommentResponse> GetCommentsAsync(int count, string token, CancellationTokenSource cancellationTokenSource = null)
        {
            // Fanburst does not support comments
            return await Task.Run(() => new BaseTrack.CommentResponse { Comments = null, Token = "" });
        }
    }
}
