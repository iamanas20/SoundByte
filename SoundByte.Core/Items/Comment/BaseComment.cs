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
using System.Diagnostics.CodeAnalysis;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Items.Comment
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class BaseComment : BaseItem
    {
        /// <summary>
        ///     What service this playlist belongs to.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        ///     The ID of the comment. Used to perform tasks on
        ///     the comment.
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
        ///     The body of the comment.
        /// </summary>
        public string Body
        {
            get => _body;
            set
            {
                if (value == _body)
                    return;

                _body = value;
                UpdateProperty();
            }
        }
        private string _body;

        /// <summary>
        ///     The date and time that this comment was created,
        /// </summary>
        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (value == _createdAt)
                    return;

                _createdAt = value;
                UpdateProperty();
            }
        }
        private DateTime _createdAt;

        /// <summary>
        ///     The time this comment was commented at on the track. Only
        ///     really used for SoundCloud tracks.
        /// </summary>
        public TimeSpan CommentTime
        {
            get => _commentTime;
            set
            {
                if (value == _commentTime)
                    return;

                _commentTime = value;
                UpdateProperty();
            }
        }
        private TimeSpan _commentTime;

        /// <summary>
        ///     The user who commented
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
    }
}
