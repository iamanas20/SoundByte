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

        public string Kind { get; set; }

        public string Link { get; set; }

        public bool IsLive { get; set; }

        public string AudioStreamUrl { get; set; }

        public string VideoStreamUrl { get; set; }

        public string ArtworkUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastPlaybackDate { get; set; }

        public double LikeCount { get; set; }

        public double DislikeCount { get; set; }

        public double ViewCount { get; set; }

        public double CommentCount { get; set; }

        public string Genre { get; set; }

        public bool IsLiked { get; set; }

        public BaseUser User { get; set; }

        public async Task<CommentResponse> GetCommentsAsync(int count, string token, CancellationTokenSource cancellationTokenSource = null)
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

        public async void Like()
        {
            // We have already liked the track
            if (IsLiked)
                return;

            bool hasLiked;

            switch (ServiceType)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                     hasLiked = await new SoundCloudTrack(Id).LikeAsync();
                    break;
                case ServiceType.Fanburst:
                    hasLiked = await new FanburstTrack(Id).LikeAsync();
                    break;
                case ServiceType.YouTube:
                    hasLiked = await new YouTubeTrack(Id).LikeAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            IsLiked = hasLiked;
        }

        public async void Unlike()
        {
            // We have already unliked the track
            if (!IsLiked)
                return;

            bool hasUnliked;

            switch (ServiceType)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    hasUnliked = await new SoundCloudTrack(Id).UnlikeAsync();
                    break;
                case ServiceType.Fanburst:
                    hasUnliked = await new FanburstTrack(Id).UnlikeAsync();
                    break;
                case ServiceType.YouTube:
                    hasUnliked = await new YouTubeTrack(Id).UnlikeAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            IsLiked = !hasUnliked;
        }

        public class CommentResponse
        {
            public List<BaseComment> Comments { get; set; }
            public string Token { get; set; }
        }
    }
}
