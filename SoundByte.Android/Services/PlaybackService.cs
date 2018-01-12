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
using Android.App;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Uri = Android.Net.Uri;

namespace SoundByte.Android.Services
{
    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop })]
    public class PlaybackService : Service, AudioManager.IOnAudioFocusChangeListener
    {
        private MediaPlayer _player;
        private AudioManager _audioManager;
        private WifiManager _wifiManager;
        private WifiManager.WifiLock _wifiLock;
        private bool _isPaused;

        //Actions
        public const string ActionPlay = "net.gridentertainment.soundbyte.android.action.PLAY";
        public const string ActionPause = "net.gridentertainment.soundbyte.android.action.PAUSE";
        public const string ActionStop = "net.gridentertainment.soundbyte.android.action.STOP";

        private const int NotificationId = 1;

        /// <summary>
        /// On create simply detect some of our managers
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            //Find our audio and notificaton managers
            _audioManager = (AudioManager)GetSystemService(AudioService);
            _wifiManager = (WifiManager)GetSystemService(WifiService);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            switch (intent.Action)
            {
                case ActionPlay: Play(intent.GetStringExtra("URL")); break;
                case ActionStop: Stop(); break;
                case ActionPause: Pause(); break;
            }
            //Set sticky as we are a long running operation
            return StartCommandResult.Sticky;
        }

        private void IntializePlayer()
        {
            _player = new MediaPlayer();

            //Tell our player to stream music
            _player.SetAudioStreamType(Stream.Music);

            _player.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);

            //When we have prepared the song start playback
            _player.Prepared += (sender, args) => _player.Start();

            //When we have reached the end of the song stop ourselves, however you could signal next track here.
            _player.Completion += (sender, args) => Stop();
            _player.Error += (sender, args) => {
                //playback error
                Console.WriteLine("Error in playback resetting: " + args.What);
                Stop();//this will clean up and reset properly.
            };
        }

        private async void Play(string track)
        {
            if (_isPaused && _player != null)
            {
                _isPaused = false;
                //We are simply paused so just start again
                _player.Start();
                StartForeground();
                return;
            }

            if (_player == null)
            {
                IntializePlayer();
            }

            if (_player.IsPlaying)
                return;

            try
            {
                await _player.SetDataSourceAsync(ApplicationContext, Uri.Parse(track));

                var focusResult = _audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
                if (focusResult != AudioFocusRequest.Granted)
                {
                    //could not get audio focus
                    Console.WriteLine("Could not get audio focus");
                }

                _player.PrepareAsync();
                AquireWifiLock();
                StartForeground();
            }
            catch (Exception ex)
            {
                //unable to start playback log error
                Console.WriteLine("Unable to start playback: " + ex);
            }

        }

        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        private void StartForeground()
        {

            var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0,
                            new Intent(ApplicationContext, typeof(MainActivity)),
                            PendingIntentFlags.UpdateCurrent);

            var notification = new Notification
            {
                TickerText = new Java.Lang.String("Song started!"),
                Icon = Resource.Drawable.abc_spinner_mtrl_am_alpha
            };
            notification.Flags |= NotificationFlags.OngoingEvent;

#pragma warning disable 618
            notification.SetLatestEventInfo(ApplicationContext, "SoundByte",
                            "Now Playing!", pendingIntent);
            StartForeground(NotificationId, notification);
#pragma warning restore 618
        }

        private void Pause()
        {
            if (_player == null)
                return;

            if (_player.IsPlaying)
                _player.Pause();

            StopForeground(true);
            _isPaused = true;
        }

        private void Stop()
        {
            if (_player == null)
                return;

            if (_player.IsPlaying)
                _player.Stop();

            _player.Reset();
            _isPaused = false;
            StopForeground(true);
            ReleaseWifiLock();
        }

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        private void AquireWifiLock()
        {
            if (_wifiLock == null)
            {
                _wifiLock = _wifiManager.CreateWifiLock(WifiMode.Full, "xamarin_wifi_lock");
            }
            _wifiLock.Acquire();
        }

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        private void ReleaseWifiLock()
        {
            if (_wifiLock == null)
                return;

            _wifiLock.Release();
            _wifiLock = null;
        }

        /// <summary>
        /// Properly cleanup of your player by releasing resources
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_player != null)
            {
                _player.Release();
                _player = null;
            }
        }

        /// <summary>
        /// For a good user experience we should account for when audio focus has changed.
        /// There is only 1 audio output there may be several media services trying to use it so
        /// we should act correctly based on this.  "duck" to be quiet and when we gain go full.
        /// All applications are encouraged to follow this, but are not enforced.
        /// </summary>
        /// <param name="focusChange"></param>
        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    if (_player == null)
                        IntializePlayer();

                    if (!_player.IsPlaying)
                    {
                        _player.Start();
                        _isPaused = false;
                    }

                    _player.SetVolume(1.0f, 1.0f);//Turn it up!
                    break;
                case AudioFocus.Loss:
                    //We have lost focus stop!
                    Stop();
                    break;
                case AudioFocus.LossTransient:
                    //We have lost focus for a short time, but likely to resume so pause
                    Pause();
                    break;
                case AudioFocus.LossTransientCanDuck:
                    //We have lost focus but should till play at a muted 10% volume
                    if (_player.IsPlaying)
                        _player.SetVolume(.1f, .1f);//turn it down!
                    break;

            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}