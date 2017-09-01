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
using SoundByte.API.Endpoints;

namespace SoundByte.API.Items.User
{
    [JsonObject]
    public class SoundCloudUser : IUser
    {
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("permalink_url")]
        public string PermalinkUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("verified")]
        public bool IsVerified { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("track_count")]
        public int? TrackCount { get; set; }

        [JsonProperty("playlist_count")]
        public int? PlaylistCount { get; set; }

        [JsonProperty("followers_count")]
        public int? FollowersCount { get; set; }

        [JsonProperty("followings_count")]
        public int? FollowingsCount { get; set; }

        public BaseUser AsBaseUser => ToBaseUser();

        public BaseUser ToBaseUser()
        {
            return new BaseUser
            {
                ServiceType = ServiceType.SoundCloud,
                Id = Id.ToString(),
                Username = Username,
                ArtworkLink = AvatarUrl,
                Country = Country,
                PermalinkUri = PermalinkUrl,
                Description = Description,
                TrackCount = TrackCount ?? 0,
                FollowersCount = FollowersCount ?? 0,
                PlaylistCount = PlaylistCount ?? 0,
                FollowingsCount = FollowingsCount ?? 0
            };
        }
    }
}
