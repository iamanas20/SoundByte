using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SoundByte.Core.Items.User
{
    /// <summary>
    ///     A universal user class that is consistent for
    ///     all service types. All elements are updateable by
    ///     the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [Table("users", Schema = "data")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BaseUser : BaseItem
    {
        /// <summary>
        ///     What service this user belongs to. Useful for
        ///     performing service specific tasks such as following.
        /// </summary>
        [Column("service_type")]
        [JsonProperty("service_type")]
        public ServiceType ServiceType { get; set; }

        /// <summary>
        ///     The SoundByte resource ID
        /// </summary>
        [Column("id")]
        [JsonProperty("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Column("user_id")]
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [Column("username")]
        [JsonProperty("username")]
        public string Username { get; set; }

        [Column("artwork_link")]
        [JsonProperty("artwork_link")]
        public string ArtworkLink { get; set; }

        [Column("country")]
        [JsonProperty("country")]
        public string Country { get; set; }

        [Column("permalink_uri")]
        [JsonProperty("permalink_uri")]
        public string PermalinkUri { get; set; }

        [Column("description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        [Column("track_count")]
        [JsonProperty("track_count")]
        public double TrackCount { get; set; }

        [Column("followers_count")]
        [JsonProperty("followers_count")]
        public double FollowersCount { get; set; }

        [Column("followings_count")]
        [JsonProperty("followings_count")]
        public double FollowingsCount { get; set; }

        [Column("playlist_count")]
        [JsonProperty("playlist_count")]
        public double PlaylistCount { get; set; }
    }
}