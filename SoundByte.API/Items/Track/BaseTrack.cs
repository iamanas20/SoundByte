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
using System.Threading.Tasks;
using SoundByte.API.Endpoints;
using SoundByte.API.Items.User;

namespace SoundByte.API.Items.Track
{
    /// <summary>
    /// A universal track class that is consistent for
    /// all service types. All elements are updateable by
    /// the UI.
    /// </summary>
    public class BaseTrack : BaseItem
    {
        /// <summary>
        /// What service this track belongs to. Useful for
        /// performing service specific tasks such as liking.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        public string Id { get; set; }
        public string Kind { get; set; }
        public string Link { get; set; }
        public string AudioStreamUrl { get; set; }
        public string VideoStreamUrl { get; set; }
        public string ArtworkUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public TimeSpan Duration { get; set; }
        public DateTime Created { get; set; }
        public double LikeCount { get; set; }
        public double DislikeCount { get; set; }
        public double ViewCount { get; set; }
        public double CommentCount { get; set; }
        public string Genre { get; set; }
        public BaseUser User { get; set; }

        #region Methods
        /// <summary>
        /// Likes the track using service specific API calls. The service
        /// must be connected for this call to work.
        /// </summary>
        /// <returns>True if track is liked, false if any errors occured.</returns>
        public async Task<bool> LikeTrackAsync()
        {

            throw new NotImplementedException();
        }
        #endregion
    }
}
