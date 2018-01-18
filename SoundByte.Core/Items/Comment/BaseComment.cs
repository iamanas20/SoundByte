using System;
using SoundByte.Core.Items.User;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SoundByte.Core.Items.Comment
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BaseComment : BaseItem
    {
        /// <summary>
        ///     What service this playlist belongs to.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        ///     The ID of the comment. Used to perform tasks on
        ///     the comment.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The body of the comment.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     The date and time that this comment was created,
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     The time this comment was commented at on the track. Only
        ///     really used for SoundCloud tracks.
        /// </summary>
        public TimeSpan CommentTime { get; set; }

        /// <summary>
        ///     The user who commented
        /// </summary>
        public BaseUser User { get; set; }
    }
}