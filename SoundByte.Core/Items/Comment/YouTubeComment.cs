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
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Items.Comment
{
    [JsonObject]
    public class YouTubeComment : IComment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("snippet")]
        public YtSnippet Snippet { get; set; }

        [JsonObject]
        public class YtSnippet
        {
            [JsonProperty("topLevelComment")]
            public YtComment Comment { get; set; }
        }

        [JsonObject]
        public class YtComment
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("snippet")]
            public YtCommentSnippet Snippet { get; set; }
        }

        [JsonObject]
        public class YtCommentSnippet
        {
            [JsonProperty("authorDisplayName")]
            public string AuthorName { get; set; }

            [JsonProperty("authorProfileImageUrl")]
            public string AuthorProfileUrl { get; set; }

            [JsonProperty("textDisplay")]
            public string CommentText { get; set; }

            [JsonProperty("publishedAt")]
            public DateTime Created { get; set; }
        }

        public BaseComment ToBaseComment()
        {
            return new BaseComment
            {
                Body = Snippet.Comment.Snippet.CommentText,
                CreatedAt = Snippet.Comment.Snippet.Created,
                Id = Snippet.Comment.Id,
                ServiceType = ServiceType.YouTube,
                Timestamp = TimeSpan.Zero,
                User = new BaseUser
                {
                    ArtworkLink = Snippet.Comment.Snippet.AuthorProfileUrl,
                    ServiceType = ServiceType.YouTube,
                    Username = Snippet.Comment.Snippet.AuthorName
                }
            };
        }
    }
}
