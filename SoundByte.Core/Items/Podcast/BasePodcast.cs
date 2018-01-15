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
        public string PodcastId
        {
            get => _podcastId;
            set
            {
                if (value == _podcastId)
                    return;

                _podcastId = value;
                UpdateProperty();
            }
        }
        private string _podcastId;

        /// <summary>
        ///     Username of the person who uploaded the podcast.
        /// </summary>
        [Column("username")]
        [JsonProperty("username")]
        public string Username
        {
            get => _username;
            set
            {
                if (value == _username)
                    return;

                _username = value;
                UpdateProperty();
            }
        }
        private string _username;

        /// <summary>
        ///     Title of the podcast.
        /// </summary>
        [Column("title")]
        [JsonProperty("title")]
        public string Title
        {
            get => _title;
            set
            {
                if (value == _title)
                    return;

                _title = value;
                UpdateProperty();
            }
        }
        private string _title;

        /// <summary>
        ///     Url to the podcast feed.
        /// </summary>
        [Column("feed_url")]
        [JsonProperty("feed_url")]
        public string FeedUrl
        {
            get => _feedUrl;
            set
            {
                if (value == _feedUrl)
                    return;

                _feedUrl = value;
                UpdateProperty();
            }
        }
        private string _feedUrl;

        /// <summary>
        ///     Url to the artwork.
        /// </summary>
        [Column("artwork_url")]
        [JsonProperty("artwork_url")]
        public string ArtworkUrl
        {
            get => _artworkUrl;
            set
            {
                if (value == _artworkUrl)
                    return;

                _artworkUrl = value;
                UpdateProperty();
            }
        }
        private string _artworkUrl;

        /// <summary>
        ///     Number of tracks in the podcast.
        /// </summary>
        [Column("track_count")]
        [JsonProperty("track_count")]
        public int TrackCount
        {
            get => _trackCount;
            set
            {
                if (value == _trackCount)
                    return;

                _trackCount = value;
                UpdateProperty();
            }
        }
        private int _trackCount;

        /// <summary>
        ///    Date and time this item was created.
        /// </summary>
        [Column("created")]
        [JsonProperty("created")]
        public DateTime Created
        {
            get => _created;
            set
            {
                if (value == _created)
                    return;

                _created = value;
                UpdateProperty();
            }
        }
        private DateTime _created;

        /// <summary>
        ///    Date and time this item was created.
        /// </summary>
        [Column("genre")]
        [JsonProperty("genre")]
        public string Genre
        {
            get => _genre;
            set
            {
                if (value == _genre)
                    return;

                _genre = value;
                UpdateProperty();
            }
        }
        private string _genre;
    }
}