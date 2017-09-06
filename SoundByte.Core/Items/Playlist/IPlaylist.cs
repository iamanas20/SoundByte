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

namespace SoundByte.Core.Items.Playlist
{
    /// <summary>
    /// Extend custom service playlist classes
    /// off of this interface.
    /// </summary>
    public interface IPlaylist
    {
        /// <summary>
        /// Convert the service specific track implementation to a
        /// universal implementation. Overide this method and provide
        /// the correct mapping between the service specific and universal
        /// classes.
        /// </summary>
        /// <returns>A base playlist item.</returns>
        BasePlaylist ToBasePlaylist();
    }
}
