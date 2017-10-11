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
using System.Threading;

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
        private string _id;

        public string Kind { get; set; }
        private string _kind;

        public string Link { get; set; }
        private string _link;

        public bool IsLive { get; set; }
        private bool _isLive;

        public string AudioStreamUrl { get; set; }
        private string _audioStreamurl;

        public string VideoStreamUrl { get; set; }
        private string _videoStreamUrl;

        public string ArtworkUrl { get; set; }
        private string _artworkUrl;

        public string Title { get; set; }
        private string _title;

        public string Description { get; set; }
        private string _description;

        public TimeSpan Duration { get; set; }
        private TimeSpan _duration;

        public DateTime Created { get; set; }
        private DateTime _created;

        public DateTime LastPlaybackDate { get; set; }
        private DateTime _lastPlaybackDate;

        public double LikeCount { get; set; }
        private double _likeCount;

        public double DislikeCount { get; set; }
        private double _dislikeCount;

        public double ViewCount { get; set; }
        private double _viewCount;

        public double CommentCount { get; set; }
        private double _commentCount;

        public string Genre { get; set; }
        private string _genre;

        public BaseUser User { get; set; }
        private BaseUser _user;

        public async Task<(IEnumerable<BaseComment> Comments, string Token)> GetCommentsAsync(uint count, string token, CancellationTokenSource cancellationTokenSource = null)
        {
            // Always at least 10 comments.
            if (count <= 10)
                count = 10;

            switch (ServiceType)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    return await new SoundCloudTrack(Id).GetCommentsAsync(count, token, cancellationTokenSource);
                case ServiceType.Fanburst:
                    return await new FanburstTrack(Id).GetCommentsAsync(count, token, cancellationTokenSource);
                case ServiceType.YouTube:
                    return await new YouTubeTrack(Id).GetCommentsAsync(count, token, cancellationTokenSource);
                default:
                    throw new ArgumentOutOfRangeException();
            }   
        }
    }
}
