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
using SoundByte.API.Items.Track;
using SoundByte.API.Items.User;

namespace SoundByte.API.Items.Playlist
{
    /// <summary>
    /// A universal playlist class that is consistent for
    /// all service types. All elements are updateable by
    /// the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class BasePlaylist
    {
        public string Id { get; set; }

        /// <summary>
        /// What service this playlist belongs to.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        public TimeSpan Duration { get; set; }

        public string Title { get; set; }

        public string Genre { get; set; }

        public string SecretToken { get; set; }

        public string Description { get; set; }

        public IEnumerable<BaseTrack> Tracks { get; set; }

        public DateTime CreationDate { get; set; }

        public string ArtworkLink { get; set; }

        public BaseUser User { get; set; }

        /// <summary>
        ///     Used by SoundByte to determine if the track is in a set
        /// </summary>
        public bool IsTrackInInternalSet { get; set; }

        public double LikesCount { get; set; }

        public double TrackCount { get; set; }
    }
}
