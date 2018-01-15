using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SoundByte.Core.Items.Generic
{
    /// <summary>
    ///     An item that can be muiltiple things
    /// </summary>
    public class GroupedItem
    {
        public ItemType Type { get; set; }

        public BaseUser User { get; set; }

        public BasePlaylist Playlist { get; set; }

        public BaseTrack Track { get; set; }

        public PodcastShow Podcast { get; set; }
    }
}
