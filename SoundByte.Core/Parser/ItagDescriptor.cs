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

using SoundByte.Core.Models.MediaStreams;

namespace SoundByte.Core.Parser
{
    internal class ItagDescriptor
    {
        public Container Container { get; }

        public AudioEncoding? AudioEncoding { get; }

        public VideoEncoding? VideoEncoding { get; }

        public VideoQuality? VideoQuality { get; }

        public ItagDescriptor(Container container, AudioEncoding? audioEncoding, VideoEncoding? videoEncoding, VideoQuality? videoQuality)
        {
            Container = container;
            AudioEncoding = audioEncoding;
            VideoEncoding = videoEncoding;
            VideoQuality = videoQuality;
        }
    }
}