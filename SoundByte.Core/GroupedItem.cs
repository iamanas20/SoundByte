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

using SoundByte.Core.Items;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;

namespace SoundByte.Core
{
    /// <summary>
    /// An item that can be muiltiple things
    /// </summary>
    public class GroupedItem
    {
        public ItemType Type { get; set; }

        public BaseUser User { get; set; }

        public BasePlaylist Playlist { get; set; }

        public BaseTrack Track { get; set; }

        public PodcastShow PodcastShow { get; set; }

        public PodcastEpisode PodcastEpisode { get; set; }
    }
}
