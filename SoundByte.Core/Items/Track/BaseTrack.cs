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
using System.Threading.Tasks;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Items.Track
{
    /// <summary>
    /// A universal track class that is consistent for
    /// all service types. All elements are updateable by
    /// the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
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


        public async Task<(List<BaseComment> Comments, string Token)> GetCommentsAsync(uint count, string token)
        {
            // Always at least 10 comments.
            if (count <= 10)
                count = 10;

            switch (ServiceType)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    return await new SoundCloudTrack(Id).GetCommentsAsync(count, token);
                case ServiceType.Fanburst:
                    return await new FanburstTrack(Id).GetCommentsAsync(count, token);
                case ServiceType.YouTube:
                    return await new YouTubeTrack(Id).GetCommentsAsync(count, token);
                default:
                    throw new ArgumentOutOfRangeException();
            }   
        }
    }
}
