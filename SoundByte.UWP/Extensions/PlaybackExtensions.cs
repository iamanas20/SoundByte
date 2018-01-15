using Windows.Media.Core;
using SoundByte.Core.Items.Track;

namespace SoundByte.UWP.Extensions
{
    public static class PlaybackExtensions
    {
        public static MediaSource AsMediaSource(this BaseTrack track, MediaSource baseMediaSource)
        {
            baseMediaSource.CustomProperties["SOUNDBYTE_ITEM"] = track;
            return baseMediaSource;
        }

        public static BaseTrack AsBaseTrack(this MediaSource source)
        {
            return source.CustomProperties["SOUNDBYTE_ITEM"] as  BaseTrack;
        }
    }
}
