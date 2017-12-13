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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.User;
using System.Threading;
using SoundByte.Core.Services;
using YoutubeExplode.Models.MediaStreams;

// ReSharper disable CompareOfFloatsByEqualityOperator

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
        /// Get the audio stream for this item
        /// </summary>
        /// <returns></returns>
        public async Task<Uri> GetAudioStreamAsync()
        {
            // Get the appropriate client Ids
            var service = SoundByteV3Service.Current.Services.FirstOrDefault(x => x.Service == ServiceType);

            if (service == null)
                throw new Exception("Oh shit, this should like, never be null dude. You should probably direct message me on twitter :D (@dominicjmaas)");

            switch (ServiceType)
            {
                case ServiceType.Fanburst:
                    AudioStreamUrl = $"https://api.fanburst.com/tracks/{Id}/stream?client_id={service.ClientId}";
                    break;
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:

                    // If we have not hit the rate limit for our current main key, use that key
                    if (await SoundCloudApiCheck($"https://api.soundcloud.com/tracks/{Id}/stream?client_id={service.ClientId}"))
                    {
                        AudioStreamUrl = $"https://api.soundcloud.com/tracks/{Id}/stream?client_id={service.ClientId}";
                    }

                    // The key is invalid, loop through the other keys
                    foreach (var key in service.ClientIds)
                    {
                        if (!await SoundCloudApiCheck($"https://api.soundcloud.com/tracks/{Id}/stream?client_id={key}"))
                            continue;

                        AudioStreamUrl = $"https://api.soundcloud.com/tracks/{Id}/stream?client_id={key}";
                        break;
                    }
                    break;
                case ServiceType.YouTube:
                    // Get the video streams
                    var client = new YoutubeExplode.YoutubeClient();
                    var mediaStreams = await client.GetVideoMediaStreamInfosAsync(Id);

                    // Set the audio stream URL to the highest quality
                    // TODO: Set it at an alright quality.
                    AudioStreamUrl = mediaStreams.Audio.OrderBy(q => q.AudioEncoding).Last()?.Url;

                    // 720p is max quality we want
                    // If 720p does not exit, set it to the
                    // higest this video supports.
                    VideoStreamUrl = mediaStreams.Video
                        .FirstOrDefault(x => x.VideoQuality == VideoQuality.High720)?.Url;

                    if (string.IsNullOrEmpty(VideoStreamUrl))
                        VideoStreamUrl = mediaStreams.Video.OrderBy(s => s.VideoQuality).Last()?.Url;

                    if (IsLive)
                    {
                        // Wait for lib to update
                    //    VideoStreamUrl = mediaStreams.HlsLiveStreamUrl;
                    //    AudioStreamUrl = mediaStreams.HlsLiveStreamUrl;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Uri(AudioStreamUrl);
        }

        /// <summary>
        /// Perform checks with the soundcloud api key
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<bool> SoundCloudApiCheck(string url)
        {
            try
            {
                // Create the client
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }))
                {
                    // No Auth for this
                    client.DefaultRequestHeaders.Authorization = null;

                    using (var webRequest = await client.GetAsync(new Uri(Uri.EscapeUriString(url))))
                    {
                        return webRequest.IsSuccessStatusCode;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// What service this track belongs to. Useful for
        /// performing service specific tasks such as liking.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        /// Id of track
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
        /// Kind of track
        /// </summary>
        public string Kind
        {
            get => _kind;
            set
            {
                if (value == _kind)
                    return;

                _kind = value;
                UpdateProperty();
            }
        }
        private string _kind;

        /// <summary>
        /// Link to the track
        /// </summary>
        public string Link
        {
            get => _link;
            set
            {
                if (value == _link)
                    return;

                _link = value;
                UpdateProperty();
            }
        }
        private string _link;

        /// <summary>
        /// Is the track currently live (used for YouTube videos)
        /// </summary>
        public bool IsLive
        {
            get => _isLive;
            set
            {
                if (value == _isLive)
                    return;

                _isLive = value;
                UpdateProperty();
            }
        }
        private bool _isLive;

        /// <summary>
        /// Url to the audio stream
        /// </summary>
        public string AudioStreamUrl
        {
            get => _audioStreamUrl;
            set
            {
                if (value == _audioStreamUrl)
                    return;

                _audioStreamUrl = value;
                UpdateProperty();
            }
        }
        private string _audioStreamUrl;

        /// <summary>
        /// Url to the video stream
        /// </summary>
        public string VideoStreamUrl
        {
            get => _videoStreamUrl;
            set
            {
                if (value == _videoStreamUrl)
                    return;

                _videoStreamUrl = value;
                UpdateProperty();
            }
        }
        private string _videoStreamUrl;

        /// <summary>
        /// Artwork link
        /// </summary>
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
        /// Title of the track
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
        /// Description of the track
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
        /// How long is the track
        /// </summary>
        public TimeSpan Duration
        {
            get => _duation;
            set
            {
                if (value == _duation)
                    return;

                _duation = value;
                UpdateProperty();
            }
        }
        private TimeSpan _duation;

        /// <summary>
        /// Date and time the track was created/uploaded
        /// </summary>
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
        /// Last time the user played back this track
        /// </summary>
        public DateTime LastPlaybackDate
        {
            get => _lastPlaybackDate;
            set
            {
                if (value == _lastPlaybackDate)
                    return;

                _lastPlaybackDate = value;
                UpdateProperty();
            }
        }
        private DateTime _lastPlaybackDate;

        /// <summary>
        /// Amount of likes
        /// </summary>
        public double LikeCount
        {
            get => _likeCount;
            set
            {
                if (value == _likeCount)
                    return;

                _likeCount = value;
                UpdateProperty();
            }
        }
        private double _likeCount;

        /// <summary>
        /// Amount of dislikes (if supported - YouTube)
        /// </summary>
        public double DislikeCount
        {
            get => _dislikeCount;
            set
            {
                if (value == _dislikeCount)
                    return;

                _dislikeCount = value;
                UpdateProperty();
            }
        }
        private double _dislikeCount;

        /// <summary>
        /// Amount of views
        /// </summary>
        public double ViewCount
        {
            get => _viewCount;
            set
            {
                if (value == _viewCount)
                    return;

                _viewCount = value;
                UpdateProperty();
            }
        }
        private double _viewCount;

        /// <summary>
        /// Amount of comments
        /// </summary>
        public double CommentCount
        {
            get => _commentCount;
            set
            {
                if (value == _commentCount)
                    return;

                _commentCount = value;
                UpdateProperty();
            }
        }
        private double _commentCount;

        /// <summary>
        ///     Track Genre
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
        ///     Has this track been liked
        /// </summary>
        public bool IsLiked
        {
            get => _isLiked;
            set
            {
                if (value == _isLiked)
                    return;

                _isLiked = value;
                UpdateProperty();
            }
        }
        private bool _isLiked;

        /// <summary>
        ///     The Track User
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

        #region Methods
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

        public  void ToggleLike()
        {
            if (IsLiked)
            {
                Unlike();
            }
            else
            {
                Like();
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
        #endregion

        public class CommentResponse
        {
            public List<BaseComment> Comments { get; set; }
            public string Token { get; set; }
        }
    }
}
