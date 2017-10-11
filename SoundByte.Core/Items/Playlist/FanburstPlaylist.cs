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

using Newtonsoft.Json;
using SoundByte.Core.Items.User;
using System;

namespace SoundByte.Core.Items.Playlist
{
    [JsonObject]
    public class FanburstPlaylist : IPlaylist
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("tracks_count")]
        public int TracksCount { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("user")]
        public FanburstUser User { get; set; }

        public BasePlaylist ToBasePlaylist()
        {
            return new BasePlaylist
            {
                ServiceType = ServiceType.SoundCloud,
                Id = Id,
                Duration = TimeSpan.FromMilliseconds(0),
                Title = Title,
                Genre = "Unknown",
                CreationDate = PublishedAt,
                ArtworkLink = ImageUrl,
                User = User.ToBaseUser(),
                LikesCount = 0,
                TrackCount = TracksCount
            };
        }
    }
}