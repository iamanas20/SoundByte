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
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using JetBrains.Annotations;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;
using YoutubeExplode;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     UWP implementation of the Playback Service
    /// </summary>
    public partial class PlaybackV2Service : IPlaybackService
    {
        #region Delegates
        public delegate void TrackChangedEventHandler(BaseTrack newTrack);

        public delegate void StateChangedEventHandler(MediaPlaybackState mediaPlaybackState);
        #endregion

        #region Events
        /// <summary>
        ///     This event is fired when the current track changes.
        /// </summary>
        public event TrackChangedEventHandler OnTrackChange;

        /// <summary>
        ///     This event is fired when the current playback state changes
        ///     (e.g paused, playing, stopped, opening etc.)
        /// </summary>
        public event StateChangedEventHandler OnStateChange;
        #endregion

        #region Private Variables
        /// <summary>
        ///     Used for working with YouTube video streams. This is a shared
        ///     instance to increase performance.
        /// </summary>
        private YoutubeClient _youTubeClient;

        /// <summary>
        ///     Shared media player used throughout the app.
        /// </summary>
        private MediaPlayer _mediaPlayer;

        /// <summary>
        ///     The currently playing track
        /// </summary>
        [CanBeNull]
        private BaseTrack _currentTrack;

        #region Timers
        /// <summary>
        ///     This timer runs every 500ms to ensure that the current position,
        ///     time, remaining time, etc. variables are correct.
        /// </summary>
        private readonly DispatcherTimer _updateInformationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };

        /// <summary>
        ///     This timer has to run quite fast. It ensures that audio and video are
        ///     in sync for YouTube videos.
        /// </summary>
        private readonly DispatcherTimer _audioVideoSyncTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(75)
        };
        #endregion

        /// <summary>
        ///     Media Playback List that allows queuing of songs and 
        ///     gapless playback.
        /// </summary>
        private MediaPlaybackList MediaPlaybackList => _mediaPlayer.Source as MediaPlaybackList;
        #endregion

        #region Constructor
        /// <summary>
        /// Setup the playback service class for use.
        /// </summary>
        public PlaybackV2Service()
        {
            // Only keep 5 items open and do not auto repeat
            // as we will be loading more items once we reach the
            // end of a list (or starting over if in playlist)
            var mediaPlaybackList = new MediaPlaybackList
            {
                MaxPlayedItemsToKeepOpen = 5,
                AutoRepeatEnabled = false
            };

            // Create the media player and disable auto play
            // as we are going to use a playback list. Set the
            // source to the media playback list.
            _mediaPlayer = new MediaPlayer
            {
                AutoPlay = false,
                Source = mediaPlaybackList
            };

            // Create the youtube client used to parse YouTube streams.
            _youTubeClient = new YoutubeClient();

            // Assign event handlers
            MediaPlaybackList.CurrentItemChanged += MediaPlaybackListOnCurrentItemChanged;


        }
        #endregion

        #region Private Event Handlers
        /// <summary>
        ///     Occurs when a current media playback item changes.
        /// </summary>
        private void MediaPlaybackListOnCurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            // If there is no new item, don't do anything
            if (args.NewItem == null)
                return;

            throw new NotImplementedException();
        }
        #endregion

        #region Track Controls
        #endregion

        public void NextTrack()
        {
            throw new NotImplementedException();
        }

        public void PreviousTrack()
        {
            throw new NotImplementedException();
        }

        public void PauseTrack()
        {
            throw new NotImplementedException();
        }

        public void PlayTrack()
        {
            throw new NotImplementedException();
        }

        public void StartTrack(BaseTrack trackToPlay)
        {
            throw new NotImplementedException();
        }

        public void StartRandomTrack()
        {
            throw new NotImplementedException();
        }

        public BaseTrack GetCurrentTrack()
        {
            throw new NotImplementedException();
        }

        public void InitilizePlaylist(ISource<BaseTrack> source, string currentToken)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     UWP implementation of the Playback Service
    /// </summary>
    public partial class PlaybackV2Service
    {
        private static PlaybackV2Service _instance;

        /// <summary>
        ///     Singleton instance of <see cref="PlaybackV2Service"/>.
        /// </summary>
        public static PlaybackV2Service Instance => _instance ?? (_instance = new PlaybackV2Service());
    }
}
