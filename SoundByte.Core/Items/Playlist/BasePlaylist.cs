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
using System.Diagnostics.CodeAnalysis;
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
    public class BasePlaylist : BaseItem
    {
        /// <summary>
        ///     What service this playlist belongs to.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        ///     Id of the playlist, useful for performing 
        ///     tasks on the playlist.
        /// </summary>
        public string Id
        {
            get => _id;
            set
            {
                if (value == _id)
                    return;

                _id = value;
                UpdateProperty();
            }
        }
        private string _id;

        /// <summary>
        /// The length of the playlist (all tracks)
        /// </summary>
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
        /// List of tracks within the playlist
        /// </summary>
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
        /// Date the playlist was created.
        /// </summary>
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

        /// <summary>
        /// User who created the playlist
        /// </summary>
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
        ///     Used by SoundByte to determine if the track is in a playlist
        /// </summary>
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
        public double LikesCount
        {
            get => _likesCount;
            set
            {
                if (Math.Abs(value - _likesCount) < 0.1)
                    return;

                _likesCount = value;
                UpdateProperty();
            }
        }
        private double _likesCount;

        /// <summary>
        /// How many tracks are in this playlist
        /// </summary>
        public double TrackCount
        {
            get => _likesCount;
            set
            {
                if (Math.Abs(value - _trackCount) < 0.1)
                    return;

                _trackCount = value;
                UpdateProperty();
            }
        }
        private double _trackCount;
    }
}
