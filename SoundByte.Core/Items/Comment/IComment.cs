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

namespace SoundByte.Core.Items.Comment
{
    /// <summary>
    /// Extend custom service comment classes
    /// off of this interface.
    /// </summary>
    public interface IComment
    {
        /// <summary>
        /// Convert the service specific comment implementation to a
        /// universal implementation. Overide this method and provide
        /// the correct mapping between the service specific and universal
        /// classes.
        /// </summary>
        /// <returns>A base comment item.</returns>
        BaseComment ToBaseComment();
    }
}
