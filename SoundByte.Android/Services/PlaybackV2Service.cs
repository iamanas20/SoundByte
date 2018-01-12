using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Net.Wifi;
using Android.OS;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;

namespace SoundByte.Android.Services
{
    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionNext, ActionPrevious })]
    public class PlaybackV2Service : Service, AudioManager.IOnAudioFocusChangeListener, IPlaybackService
    {
        // Accessing the background service
        private const string ActionPlay = "net.gridentertainment.soundbyte.android.action.PLAY_TRACK";
        private const string ActionPause = "net.gridentertainment.soundbyte.android.action.PAUSE_TRACK";

        private const string ActionNext = "net.gridentertainment.soundbyte.android.action.NEXT_TRACK";
        private const string ActionPrevious = "net.gridentertainment.soundbyte.android.action.PREVIOUS_TRACK";

        private const int NotificationId = 1;

        // Used for keeping track of playback items
        private List<BaseTrack> _playbackList = new List<BaseTrack>();
        private BaseTrack _currentItem;

        // Used for native android playback
        private MediaPlayer _player;
        private AudioManager _audioManager;
        private WifiManager _wifiManager;
        private WifiManager.WifiLock _wifiLock;


        #region Android Specific Methods

        public PlaybackV2Service()
        {
            _player = new MediaPlayer();
            _player.SetAudioStreamType(Stream.Music);
            _player.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);

        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            throw new NotImplementedException();
        }


        private void InternalPlayTrack()
        {
            
        }

        private void InternalPauseTrack()
        {
            
        }

        private void InternalPreviousTrack()
        {
            
        }

        private void InternalNextTrack()
        {
            
        }

        /// <summary>
        ///     On create simply detect some of our managers
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            //Find our audio and notificaton managers
            _audioManager = (AudioManager)GetSystemService(AudioService);
            _wifiManager = (WifiManager)GetSystemService(WifiService);
        }

        /// <summary>
        ///     Handles background playback options
        /// </summary>
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            switch (intent.Action)
            {
                case ActionPlay: InternalPlayTrack(); break;
                case ActionPause: InternalPauseTrack(); break;
                case ActionNext: InternalNextTrack(); break;
                case ActionPrevious: InternalPreviousTrack(); break;

            }
            //Set sticky as we are a long running operation
            return StartCommandResult.Sticky;
        }

        #endregion


        public void ShufflePlaylist(bool shuffle)
        {
            throw new NotImplementedException();
        }

        public void MuteTrack(bool mute)
        {
            throw new NotImplementedException();
        }

        public void SetTrackVolume(double volume)
        {
            throw new NotImplementedException();
        }

        public void SetTrackPosition(TimeSpan value)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetTrackPosition()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetTrackDuration()
        {
            throw new NotImplementedException();
        }

        public void RepeatTrack(bool repeat)
        {
            throw new NotImplementedException();
        }

        public void NextTrack()
        {
            var intent = new Intent(ActionNext);
            intent.SetPackage("net.gridentertainment.soundbyte.android");
            StartService(intent);
        }

        public void PreviousTrack()
        {
            var intent = new Intent(ActionPrevious);
            intent.SetPackage("net.gridentertainment.soundbyte.android");
            StartService(intent);
        }

        public void PauseTrack()
        {
            var intent = new Intent(ActionPause);
            intent.SetPackage("net.gridentertainment.soundbyte.android");
            StartService(intent);
        }

        public void PlayTrack()
        {
            var intent = new Intent(ActionPlay);
            intent.SetPackage("net.gridentertainment.soundbyte.android");
            StartService(intent);
        }

        public Task StartTrackAsync(BaseTrack trackToPlay)
        {
            throw new NotImplementedException();
        }

        public Task StartRandomTrackAsync()
        {
            throw new NotImplementedException();
        }

        public BaseTrack GetCurrentTrack()
        {
            throw new NotImplementedException();
        }

        public Task<PlaybackInitilizeResponse> InitilizePlaylistAsync<T>(IEnumerable<BaseTrack> playlist = null, string token = null) where T : ISource<BaseTrack>
        {
            throw new NotImplementedException();
        }
    }
}