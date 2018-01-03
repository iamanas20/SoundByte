/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
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
    ///     Used for exception handling within the app. Supports an 
    ///     error title and message.
    /// </summary>
    public class SoundByteException : Exception
    {
        public SoundByteException(string title, string description) : base(
            string.Format("Title: {0}, Description: {1}", title, description))
        {
            ErrorTitle = title;
            ErrorDescription = description;
        }

        /// <summary>
        ///     Title of the error message
        /// </summary>
        public string ErrorTitle { get; }

        /// <summary>
        ///     A description of the error message
        /// </summary>
        public string ErrorDescription { get; }
    }
}