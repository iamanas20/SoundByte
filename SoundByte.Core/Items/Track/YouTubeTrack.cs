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
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using System.Threading;
using SoundByte.Core.Converters;
using System.Xml;

namespace SoundByte.Core.Items.Track
{
    [JsonObject]
    public class YouTubeTrack : ITrack
    {
        public YouTubeTrack()
        {
        }

        public YouTubeTrack(string id)
        {
            Id = new YouTubeId { VideoId = id };
        }

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

        [JsonObject]
        public class YouTubeContentDetails
        {
            [JsonProperty("duration")]
            public string Duration { get; set; }
        }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        [JsonConverter(typeof(YouTubeIDConverter))]
        public YouTubeId Id { get; set; }

        [JsonProperty("snippet")]
        public YouTubeSnippet Snippet { get; set; }

        [JsonProperty("contentDetails")]
        public YouTubeContentDetails ContentDetails { get; set; }

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
                Duration =  (ContentDetails != null) ? XmlConvert.ToTimeSpan(ContentDetails.Duration) : TimeSpan.FromMilliseconds(0),
                Genre = "YouTube",
                User = new BaseUser
                {
                    Id = Snippet.ChannelId,
                    Username = Snippet.ChannelTitle
                }
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

        public async Task<(IEnumerable<BaseComment> Comments, string Token)> GetCommentsAsync(uint count, string token, CancellationTokenSource cancellationTokenSource = null)
        {
            // Grab a list of YouTube comments
            var youTubeComments = await SoundByteV3Service.Current.GetAsync<YouTubeCommentHolder>(ServiceType.YouTube,
                "commentThreads", new Dictionary<string, string>
                {
                    {"maxResults", count.ToString()},
                    { "part", "snippet"},
                    { "videoId", Id.VideoId},
                    { "pageToken", token }
                }, cancellationTokenSource);

            // Convert our list of YouTube comments to base comments
            var baseCommentList = new List<BaseComment>();
            youTubeComments.Items.ForEach(x => baseCommentList.Add(x.ToBaseComment()));

            // Return the data
            return (baseCommentList, youTubeComments.NextPageToken);
        }

        [JsonObject]
        public class YouTubeCommentHolder
        {
            [JsonProperty("nextPageToken")]
            public string NextPageToken { get; set; }

            [JsonProperty("items")]
            public List<YouTubeComment> Items { get; set; }
        }
    }
}
