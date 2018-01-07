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
using Windows.UI.Xaml;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.ViewModels
{
    public class PlaybackViewModel : BaseViewModel
    {
        #region Getters and Setters
        /// <summary>
        /// The current playing track
        /// </summary>
        [CanBeNull]
        public BaseTrack CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (_currentTrack == value)
                    return;

                _currentTrack = value;
                UpdateProperty();
            }
        }
        private BaseTrack _currentTrack;

        /// <summary>
        ///     The amount of time spent listening to the track
        /// </summary>
        public string TimeListened
        {
            get => _timeListened;
            set
            {
                if (_timeListened == value)
                    return;

                _timeListened = value;
                UpdateProperty();
            }
        }
        private string _timeListened = "00:00";

        /// <summary>
        ///     The amount of time remaining
        /// </summary>
        public string TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                if (_timeRemaining == value)
                    return;

                _timeRemaining = value;
                UpdateProperty();
            }
        }
        private string _timeRemaining = "-00:00";

        /// <summary>
        ///     The current slider value
        /// </summary>
        public double CurrentTimeValue
        {
            get => _currentTimeValue;
            set
            {
                _currentTimeValue = value;
                UpdateProperty();
            }
        }
        private double _currentTimeValue;

        /// <summary>
        ///     The max slider value
        /// </summary>
        public double MaxTimeValue
        {
            get => _maxTimeValue;
            private set
            {
                _maxTimeValue = value;
                UpdateProperty();
            }
        }
        private double _maxTimeValue = 100;

        /// <summary>
        ///     The current text for the volume icon
        /// </summary>
        public string VolumeIcon
        {
            get => _volumeIcon;
            private set
            {
                if (_volumeIcon == value)
                    return;

                _volumeIcon = value;
                UpdateProperty();
            }
        }
        private string _volumeIcon = "\uE767";

        /// <summary>
        ///     The content on the play_pause button
        /// </summary>
        public string PlayButtonContent
        {
            get => _playButtonContent;
            set
            {
                if (_playButtonContent == value)
                    return;

                _playButtonContent = value;
                UpdateProperty();
            }
        }
        private string _playButtonContent = "\uE769";

        #endregion

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

        public PlaybackViewModel()
        {
            // Bind the methods that we need
            PlaybackV2Service.Instance.OnStateChange += Instance_OnStateChange;
            PlaybackV2Service.Instance.OnTrackChange += InstanceOnOnTrackChange;
        }

        #region Track Control Methods

        public void ToggleRepeat()
        {
            
        }

        public void ToggleShuffle()
        {
            
        }

        public void ToggleMute()
        {
            
        }

        #endregion



        #region Methods
        /// <summary>
        ///     Called when the playback service loads a new track. Used
        ///     to update the required values for the UI.
        /// </summary>
        /// <param name="newTrack"></param>
        private async void InstanceOnOnTrackChange(BaseTrack newTrack)
        {
            // Do nothing if running in the background
            if (DeviceHelper.IsBackground)
                return;

            // Run all this on the UI thread
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                // Set the new current track, updating the UI
                CurrentTrack = newTrack;

            });
        }

        private void Instance_OnStateChange(Windows.Media.Playback.MediaPlaybackState mediaPlaybackState)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override void Dispose()
        {
            // Unbind the methods that we need
            PlaybackV2Service.Instance.OnStateChange -= Instance_OnStateChange;
            PlaybackV2Service.Instance.OnTrackChange -= InstanceOnOnTrackChange;


            base.Dispose();
        }
    }
}
