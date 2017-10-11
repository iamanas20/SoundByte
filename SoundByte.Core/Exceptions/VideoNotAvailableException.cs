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

namespace SoundByte.Core.Exceptions
{
    /// <summary>
    /// Thrown when Youtube's frontend returns an error when getting video info
    /// </summary>
    public class VideoNotAvailableException : Exception
    {
        /// <summary>
        /// Error code
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Error reason
        /// </summary>
        public string Reason { get; }

        /// <inheritdoc />
        public VideoNotAvailableException(int code, string reason)
            : base("The video is not available")
        {
            Code = code;
            Reason = reason;
        }
    }
}