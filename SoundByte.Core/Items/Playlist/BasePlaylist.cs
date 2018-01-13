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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using SoundByte.Core.Items.Track;
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
        public string PlaylistId
        {
            get => _playlistId;
            set
            {
                if (value == _playlistId)
                    return;

                _playlistId = value;
                UpdateProperty();
            }
        }
        private string _playlistId;

        /// <summary>
        /// The length of the playlist (all tracks)
        /// </summary>
        [Column("duration")]
        [JsonProperty("duration")]
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                if (value == _duration)
                    return;

                _duration = value;
                UpdateProperty();
            }
        }
        private TimeSpan _duration;

        /// <summary>
        /// Playlist title
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
        /// Genre of the playlist
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

        /// <summary>
        /// Secret playlist token (used by SoundCloud)
        /// </summary>
        [Column("secret_token")]
        [JsonProperty("secret_token")]
        public string SecretToken
        {
            get => _secretToken;
            set
            {
                if (value == _secretToken)
                    return;

                _secretToken = value;
                UpdateProperty();
            }
        }
        private string _secretToken;

        /// <summary>
        /// Playlist description
        /// </summary>
        [Column("description")]
        [JsonProperty("description")]
        public string Description
        {
            get => _description;
            set
            {
                if (value == _description)
                    return;

                _description = value;
                UpdateProperty();
            }
        }
        private string _description;

        /// <summary>
        /// Date the playlist was created.
        /// </summary>
        [Column("creation_date")]
        [JsonProperty("creation_date")]
        public DateTime CreationDate
        {
            get => _creationDate;
            set
            {
                if (value == _creationDate)
                    return;

                _creationDate = value;
                UpdateProperty();
            }
        }
        private DateTime _creationDate;

        /// <summary>
        /// Playlist artwork url
        /// </summary>
        [Column("artwork_link")]
        [JsonProperty("artwork_link")]
        public string ArtworkLink
        {
            get => _artworkLink;
            set
            {
                if (value == _artworkLink)
                    return;

                _artworkLink = value;
                UpdateProperty();
            }
        }
        private string _artworkLink;

        [Column("user_id")]
        [JsonProperty("user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// User who created the playlist
        /// </summary>
        [Column("user")]
        [JsonProperty("user")]
        public BaseUser User
        {
            get => _user;
            set
            {
                if (value == _user)
                    return;

                _user = value;
                UpdateProperty();
            }
        }
        private BaseUser _user;

        /// <summary>
        ///     Tracks for this playlist
        /// </summary>
        [Column("tracks")]
        [JsonProperty("tracks")]
        public IEnumerable<BaseTrack> Tracks
        {
            get => _tracks;
            set
            {
                if (Equals(value, _tracks))
                    return;

                _tracks = value;
                UpdateProperty();
            }
        }
        private IEnumerable<BaseTrack> _tracks;

        /// <summary>
        ///     Used by SoundByte to determine if the track is in a playlist
        /// </summary>
        [NotMapped]
        public bool IsTrackInInternalSet
        {
            get => _isTrackInInternalSet;
            set
            {
                if (value == _isTrackInInternalSet)
                    return;

                _isTrackInInternalSet = value;
                UpdateProperty();
            }
        }
        private bool _isTrackInInternalSet;

        /// <summary>
        /// How many likes does this playlist have.
        /// </summary>
        [Column("likes_count")]
        [JsonProperty("likes_count")]
        public double LikesCount
        {
            get => _likesCount;
            set
            {
                _likesCount = value;
                UpdateProperty();
            }
        }
        private double _likesCount;

        /// <summary>
        /// How many tracks are in this playlist
        /// </summary>
        [Column("track_count")]
        [JsonProperty("track_count")]
        public double TrackCount
        {
            get => _trackCount;
            set
            {
                _trackCount = value;
                UpdateProperty();
            }
        }
        private double _trackCount;
    }
}
