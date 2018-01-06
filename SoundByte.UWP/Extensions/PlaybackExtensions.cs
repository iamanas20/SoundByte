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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
