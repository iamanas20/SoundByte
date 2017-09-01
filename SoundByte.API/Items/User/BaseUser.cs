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

using SoundByte.API.Endpoints;

namespace SoundByte.API.Items.User
{
    /// <summary>
    /// A universal user class that is consistent for
    /// all service types. All elements are updateable by
    /// the UI.
    /// </summary>
    public class BaseUser : BaseItem
    {
        /// <summary>
        /// What service this user belongs to. Useful for
        /// performing service specific tasks such as following.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        public string Id { get; set; }
        public string Username { get; set; }
        public string ArtworkLink { get; set; }
        public string Country { get; set; }
        public string PermalinkUri { get; set; }
        public string Description { get; set; }
        public double TrackCount { get; set; }
        public double FollowersCount { get; set; }
        public double PlaylistCount { get; set; }
        public double FollowingsCount { get; set; }
    }
}
