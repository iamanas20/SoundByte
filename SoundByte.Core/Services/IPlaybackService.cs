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

using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;

namespace SoundByte.Core.Services
{
    /// <summary>
    /// Handles background media playback in an app
    /// </summary>
    public interface IPlaybackService
    {
        /// <summary>
        /// Skip to the next track in the playlist
        /// </summary>
        void NextTrack();

        /// <summary>
        /// Play the previous track in the playlist
        /// </summary>
        void PreviousTrack();

        /// <summary>
        /// Pause the current track
        /// </summary>
        void PauseTrack();

        /// <summary>
        /// Plays the current track
        /// </summary>
        void PlayTrack();

        void InitilizePlaylist(ISource<BaseTrack> source, string currentToken);


    }
}
