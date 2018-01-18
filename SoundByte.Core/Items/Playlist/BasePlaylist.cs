using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Items.Playlist
{
    /// <summary>
    ///     A universal playlist class that is consistent for
    ///     all service types. All elements are updateable by
    ///     the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [Table("playlists", Schema = "data")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BasePlaylist : BaseItem
    {
        /// <summary>
        ///     What service this playlist belongs to.
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

        /// <summary>
        ///     Id of the playlist, useful for performing 
        ///     tasks on the playlist.
        /// </summary>
        [Column("playlist_id")]
        [JsonProperty("playlist_id")]
        public string PlaylistId { get; set; }

        /// <summary>
        /// The length of the playlist (all tracks)
        /// </summary>
        [Column("duration")]
        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Playlist title
        /// </summary>
        [Column("title")]
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Genre of the playlist
        /// </summary>
        [Column("genre")]
        [JsonProperty("genre")]
        public string Genre { get; set; }

        /// <summary>
        /// Playlist description
        /// </summary>
        [Column("description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Date the playlist was created.
        /// </summary>
        [Column("creation_date")]
        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Playlist artwork url
        /// </summary>
        [Column("artwork_url")]
        [JsonProperty("artwork_url")]
        public string ArtworkUrl { get; set; }

        /// <summary>
        ///     Playlist thumbnail url
        /// </summary>
        [Column("thumbnail_url")]
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [Column("user_id")]
        [JsonProperty("user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// User who created the playlist
        /// </summary>
        [Column("user")]
        [JsonProperty("user")]
        public BaseUser User { get; set; }

        /// <summary>
        ///     Used by SoundByte to determine if the track is in a playlist
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsTrackInInternalSet { get; set; }

        /// <summary>
        /// How many likes does this playlist have.
        /// </summary>
        [Column("likes_count")]
        [JsonProperty("likes_count")]
        public double LikesCount { get; set; }

        /// <summary>
        /// How many tracks are in this playlist
        /// </summary>
        [Column("track_count")]
        [JsonProperty("track_count")]
        public double TrackCount { get; set; }

        /// <summary>
        ///     Custom properties you can set
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public Dictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();
    }
}
