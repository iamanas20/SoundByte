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

namespace SoundByte.Core.Items.Track
{
    [JsonObject]
    public class YouTubeTrack : ITrack
    {
        [JsonObject]
        public class YouTubeId
        {
            [JsonProperty("kind")]
            public string Kind { get; set; }

            [JsonProperty("channelId")]
            public string ChannelId { get; set; }

            [JsonProperty("videoId")]
            public string VideoId { get; set; }

            [JsonProperty("playlistId")]
            public string PlaylistId { get; set; }
        }

        [JsonObject]
        public class YouTubeThumbnailSize
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("width")]
            public int? Width { get; set; }

            [JsonProperty("height")]
            public int? Height { get; set; }
        }

        [JsonObject]
        public class YouTubeThumbnails
        {
            [JsonProperty("default")]
            public YouTubeThumbnailSize DefaultSize { get; set; }

            [JsonProperty("medium")]
            public YouTubeThumbnailSize MediumSize { get; set; }

            [JsonProperty("high")]
            public YouTubeThumbnailSize HighSize { get; set; }
        }

        [JsonObject]
        public class YouTubeSnippet
        {
            [JsonProperty("publishedAt")]
            public string PublishedAt { get; set; }

            [JsonProperty("channelId")]
            public string ChannelId { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("thumbnails")]
            public YouTubeThumbnails Thumbnails { get; set; }

            [JsonProperty("channelTitle")]
            public string ChannelTitle { get; set; }

            [JsonProperty("liveBroadcastContent")]
            public string LiveBroadcastContent { get; set; }
        }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        public YouTubeId Id { get; set; }

        [JsonProperty("snippet")]
        public YouTubeSnippet Snippet { get; set; }

        public BaseTrack AsBaseTrack => ToBaseTrack();

        /// <summary>
        /// Convert this YouTube specific track to a universal track.
        /// </summary>
        /// <returns></returns>
        public BaseTrack ToBaseTrack()
        {
            var track = new BaseTrack
            {
                ServiceType = ServiceType.YouTube,
                Id = Id.VideoId,
                Link = $"https://www.youtube.com/watch?v={Id.VideoId}",
                ArtworkUrl = Snippet.Thumbnails.HighSize.Url,
                Title = Snippet.Title,
                Description = Snippet.Description,
                Created = DateTime.Parse(Snippet.PublishedAt),
                Genre = "YouTube"
            };

            // Get the correct kind value for the
            // universal object
            switch (Id.Kind)
            {
                case "youtube#video":
                    track.Kind = "track";
                    break;
                default:
                    track.Kind = Kind;
                    break;
            }

            return track;
        }
    }
}
