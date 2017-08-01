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
using SoundByte.Core.Dialogs;
using SoundByte.Core.Services;

namespace SoundByte.Core.API.Endpoints
{
    /// <summary>
    ///     A class for holding a soundcloud track
    /// </summary>
    [JsonObject]
    public class Track : BaseTrack
    {
        #region Privates

        // Allows the user object to be updated
        private User _user;

        #endregion


        /// <summary>
        ///     The user who posted this track
        /// </summary>
        [JsonProperty("user")]
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                UpdateProperty();
            }
        }

        /// <summary>
        ///     What type of service this item belongs to
        /// </summary>
        public SoundByteService.ServiceType ServiceType { get; set; } = SoundByteService.ServiceType.SoundCloud;

        /// <summary>
        ///     The Genre of the track
        /// </summary>
        [JsonProperty("genre")]
        public string Genre { get; set; }

        /// <summary>
        ///     The ID of the track
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     What kind of track is this
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        ///     The date and time that this object was created
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     Track Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     Uri of the waveform object
        /// </summary>
        [JsonProperty("waveform_url")]
        public string WaveformUri { get; set; }

        /// <summary>
        ///     SoundCloud Link to this track
        /// </summary>
        [JsonProperty("permalink_url")]
        public string PermalinkUri { get; set; }

        /// <summary>
        ///     URL to the artwork image (JEPG)
        /// </summary>
        [JsonProperty("artwork_url")]
        public string ArtworkLink { get; set; }

        /// <summary>
        ///     The Description for the track
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Duration object of the track
        /// </summary>
        [JsonProperty("duration")]
        public int Duration { get; set; }

        /// <summary>
        ///     Amount of comments on this track
        /// </summary>
        [JsonProperty("comment_count")]
        public int? CommentCount { get; set; }

        /// <summary>
        ///     About of plays on this track
        /// </summary>
        [JsonProperty("playback_count")]
        public int? PlaybackCount { get; set; }

        /// <summary>
        ///     Amount of likes on this track
        /// </summary>
        [JsonProperty("likes_count")]
        public int? LikesCount { get; set; }

        /// <summary>
        ///     User Favourited (only auth requests)
        /// </summary>
        [JsonProperty("user_favorite")]
        public bool? UserFavorite { get; set; } = false;

        #region Methods

        /// <summary>
        ///     Open the share dialog for this track
        /// </summary>
        public async void ShareTrack()
        {
            await new ShareDialog(this).ShowAsync();
        }

        #endregion
    }
}