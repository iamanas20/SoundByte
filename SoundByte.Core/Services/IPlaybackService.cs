using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Generic;
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
        /// Shuffle or unshuffle the playlist.
        /// </summary>
        /// <param name="shuffle">True for shuffle, false for not.</param>
        void ShufflePlaylist(bool shuffle);

        /// <summary>
        ///     Mute or unmute the audio.
        /// </summary>
        /// <param name="mute">True if mute, false if not.</param>
        void MuteTrack(bool mute);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume"></param>
        void SetTrackVolume(double volume);

        void SetTrackPosition(TimeSpan value);



        TimeSpan GetTrackPosition();

        TimeSpan GetTrackDuration();

        /// <summary>
        ///     Repeat or play as normal
        /// </summary>
        /// <param name="repeat">True is repeated, false if not</param>
        void RepeatTrack(bool repeat);

        /// <summary>
        ///     Skip to the next track in the playlist
        /// </summary>
        void NextTrack();

        /// <summary>
        ///     Play the previous track in the playlist
        /// </summary>
        void PreviousTrack();

        /// <summary>
        ///     Pause the current track
        /// </summary>
        void PauseTrack();

        /// <summary>
        ///     Plays the current track
        /// </summary>
        void PlayTrack();

        /// <summary>
        ///     Start playlist at a specific track (if no track is supplied, a random track 
        ///     will be played).
        /// </summary>
        /// <param name="trackToPlay">The track to play, must exist in the playlist.</param>
        Task StartTrackAsync(BaseTrack trackToPlay);

        /// <summary>
        ///     Start playlist a random tracks
        /// </summary>
        Task StartRandomTrackAsync();

        /// <summary>
        ///     Get the current playing track (if exists)
        /// </summary>
        /// <returns>Returns the current playing track. Will be null if no tracks are playing</returns>
        [CanBeNull]
        BaseTrack GetCurrentTrack();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="playlist"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<PlaybackInitilizeResponse> InitilizePlaylistAsync<T>(IEnumerable<BaseTrack> playlist = null, string token = null) where T : ISource<BaseTrack>;
    }
}
