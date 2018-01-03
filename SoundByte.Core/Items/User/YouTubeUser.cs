/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
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
using SoundByte.Core.Converters.YouTube;
using SoundByte.Core.Items.YouTube;

namespace SoundByte.Core.Items.User
{
    [JsonObject]
    public class YouTubeUser : IUser
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        [JsonConverter(typeof(YouTubeChannelIdConverter))]
        public YouTubeId Id { get; set; }

        [JsonProperty("snippet")]
        public YouTubeSnippet Snippet { get; set; }

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
        }

        public BaseUser ToBaseUser()
        {
            return new BaseUser
            {
                ServiceType = ServiceType.YouTube,
                Id = Id.ChannelId,
                Username = Snippet.Title,
                ArtworkLink = Snippet.Thumbnails.HighSize.Url,
                Description = Snippet.Description
            };
        }
    }
}