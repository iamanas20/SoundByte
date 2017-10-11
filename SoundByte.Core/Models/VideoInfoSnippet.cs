using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundByte.Core.Models
{
    /// <summary>
    /// Partial video info
    /// </summary>
    public class VideoInfoSnippet
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Duration
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Search keywords
        /// </summary>
        public IReadOnlyList<string> Keywords { get; }

        /// <summary>
        /// Thumbnail image URL
        /// </summary>
        public string ImageThumbnailUrl => $"https://img.youtube.com/vi/{Id}/default.jpg";

        /// <summary>
        /// Medium resolution image URL
        /// </summary>
        public string ImageMediumResUrl => $"https://img.youtube.com/vi/{Id}/mqdefault.jpg";

        /// <summary>
        /// High resolution image URL
        /// </summary>
        public string ImageHighResUrl => $"https://img.youtube.com/vi/{Id}/hqdefault.jpg";

        /// <summary>
        /// Standard resolution image URL.
        /// Not always available.
        /// </summary>
        public string ImageStandardResUrl => $"https://img.youtube.com/vi/{Id}/sddefault.jpg";

        /// <summary>
        /// Highest resolution image URL.
        /// Not always available.
        /// </summary>
        public string ImageMaxResUrl => $"https://img.youtube.com/vi/{Id}/maxresdefault.jpg";

        /// <summary>
        /// View count
        /// </summary>
        public long ViewCount { get; }

        /// <inheritdoc />
        public VideoInfoSnippet(string id, string title, TimeSpan duration,
            IEnumerable<string> keywords, long viewCount)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Duration = duration >= TimeSpan.Zero ? duration : throw new ArgumentOutOfRangeException(nameof(duration));
            Keywords = keywords?.ToArray() ?? throw new ArgumentNullException(nameof(keywords));
            ViewCount = viewCount >= 0 ? viewCount : throw new ArgumentOutOfRangeException(nameof(viewCount));
        }
    }
}