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
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Items.Comment
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]

    public class BaseComment
    {
        public string Id { get; set; }

        /// <summary>
        /// What service this playlist belongs to.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }
      
        public TimeSpan Timestamp { get; set; }

        public BaseUser User { get; set; }
    }
}
