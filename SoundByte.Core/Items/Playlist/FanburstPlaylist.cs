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
                PlaylistId = Id,
                Duration = TimeSpan.FromMilliseconds(0),
                Title = Title,
                Genre = "Unknown",
                CreationDate = PublishedAt,
                ArtworkUrl = ImageUrl,
                ThumbnailUrl = ImageUrl,
                User = User.ToBaseUser(),
                LikesCount = 0,
                TrackCount = TracksCount
            };
        }
    }
}