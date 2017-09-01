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
using SoundByte.API.Items.User;

namespace SoundByte.API.Items.Track
{
    /// <summary>
    /// SoundCloud Specific Item
    /// </summary>
    [JsonObject]
    public class SoundCloudTrack : ITrack
    {
        [JsonProperty("artwork_url")]
        public string ArtworkUrl { get; set; }

        [JsonProperty("commentable")]
        public bool IsCommentable { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("download_count")]
        public int DownloadCount { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("license")]
        public string License { get; set; }

        [JsonProperty("likes_count")]
        public int LikesCount { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("permalink_url")]
        public string PermalinkUrl { get; set; }

        [JsonProperty("playback_count")]
        public int PlaybackCount { get; set; }

        [JsonProperty("@public")]
        public bool IsPublic { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("reposts_count")]
        public int RepostsCount { get; set; }

        [JsonProperty("secret_token")]
        public object SecretToken { get; set; }

        [JsonProperty("tag_list")]
        public string TagList { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("user")]
        public SoundCloudUser User { get; set; }

        /// <summary>
        /// Convert this SoundCloud specific track to a universal track.
        /// </summary>
        /// <returns></returns>
        public BaseTrack ToBaseTrack()
        {
            return new BaseTrack
            {
                ServiceType = ServiceType.SoundCloud,
                Id = Id.ToString(),
                Kind = Kind,
                Link = PermalinkUrl,
                AudioStreamUrl = string.Empty,
                VideoStreamUrl = string.Empty,
                ArtworkUrl = ArtworkUrl,
                Title = Title,
                Description = Description,
                Duration = TimeSpan.FromMilliseconds(Duration), 
                Created = DateTime.Parse(CreatedAt), 
                LikeCount = LikesCount,
                DislikeCount = 0,
                ViewCount = PlaybackCount, 
                CommentCount = CommentCount,
                Genre = Genre,
                User = User.ToBaseUser()
            };
        }
    }
}
