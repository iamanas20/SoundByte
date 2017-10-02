using System;
using System.Collections.Generic;
using System.Linq;
using SoundByte.YouTubeParser.Models.MediaStreams;

namespace SoundByte.YouTubeParser.Models
{
    /// <summary>
    /// Video info
    /// </summary>
    public class VideoInfo : VideoInfoSnippet
    {
        /// <summary>
        /// Collection of watermark URLs
        /// </summary>
        public IReadOnlyList<string> Watermarks { get; }

        /// <summary>
        /// Whether this video is publicly listed
        /// </summary>
        public bool IsListed { get; }

        /// <summary>
        /// Whether liking/disliking this video is allowed
        /// </summary>
        public bool IsRatingAllowed { get; }

        /// <summary>
        /// Whether the audio track has been muted
        /// </summary>
        public bool IsMuted { get; }

        /// <summary>
        /// Whether embedding this video on other websites is allowed
        /// </summary>
        public bool IsEmbeddingAllowed { get; }

        /// <summary>
        /// Audio-only streams available for this video
        /// </summary>
        public IReadOnlyList<AudioStreamInfo> AudioStreams { get; }

        /// <summary>
        /// Video-only streams available for this video
        /// </summary>
        public IReadOnlyList<VideoStreamInfo> VideoStreams { get; }

        /// <summary>
        /// The URL of the dash manifest (if exist, used for adaptive live streams)
        /// </summary>
        public string DashManifestUrl { get; set; }

        /// <inheritdoc />
        public VideoInfo(string id, string title, TimeSpan duration,
            IEnumerable<string> keywords, IEnumerable<string> watermarks, long viewCount, 
            bool isListed, bool isRatingAllowed, bool isMuted, bool isEmbeddingAllowed, IEnumerable<AudioStreamInfo> audioStreams,
            IEnumerable<VideoStreamInfo> videoStreams, string dashManifestUrl)
            : base(id, title, duration, keywords, viewCount)
        {
            Watermarks = watermarks?.ToArray() ?? throw new ArgumentNullException(nameof(watermarks));
            IsListed = isListed;
            IsRatingAllowed = isRatingAllowed;
            IsMuted = isMuted;
            IsEmbeddingAllowed = isEmbeddingAllowed;
            AudioStreams = audioStreams?.ToArray() ?? throw new ArgumentNullException(nameof(audioStreams));
            VideoStreams = videoStreams?.ToArray() ?? throw new ArgumentNullException(nameof(videoStreams));
            DashManifestUrl = dashManifestUrl;
        }
    }
}