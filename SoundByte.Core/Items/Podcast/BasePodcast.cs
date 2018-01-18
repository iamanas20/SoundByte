using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SoundByte.Core.Items.Podcast
{
    /// <summary>
    ///     A universal podcast class that is consistent for
    ///     all service types. All elements are updateable by
    ///     the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [Table("podcasts", Schema = "data")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BasePodcast : BaseItem
    {
        /// <summary>
        ///     What service this podcast belongs to.
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
        ///     Id of the podcast, useful for performing 
        ///     tasks on the podcast.
        /// </summary>
        [Column("podcast_id")]
        [JsonProperty("podcast_id")]
        public string PodcastId { get; set; }

        /// <summary>
        ///     Username of the person who uploaded the podcast.
        /// </summary>
        [Column("username")]
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        ///     Title of the podcast.
        /// </summary>
        [Column("title")]
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     Url to the podcast feed.
        /// </summary>
        [Column("feed_url")]
        [JsonProperty("feed_url")]
        public string FeedUrl { get; set; }

        /// <summary>
        ///     Url to the artwork.
        /// </summary>
        [Column("artwork_url")]
        [JsonProperty("artwork_url")]
        public string ArtworkUrl { get; set; }

        /// <summary>
        ///     Number of tracks in the podcast.
        /// </summary>
        [Column("track_count")]
        [JsonProperty("track_count")]
        public int TrackCount { get; set; }

        /// <summary>
        ///    Date and time this item was created.
        /// </summary>
        [Column("created")]
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        ///    Date and time this item was created.
        /// </summary>
        [Column("genre")]
        [JsonProperty("genre")]
        public string Genre { get; set; }
    }
}