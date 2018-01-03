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
    ///     Generic exception called when trying to access resources before the SoundByte
    ///     Service has loaded.
    /// </summary>
    [Serializable]
    public class SoundByteNotLoadedException : Exception
    {
        public SoundByteNotLoadedException() : base("The SoundByte Service has not been loaded. Cannot continue.")
        { }
    }
}
