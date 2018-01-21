using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Shell;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     Used for roaming content accross devices and platforms.
    /// </summary>
    public class RoamingService
    {
        private UserActivitySession _currentUserActivitySession;
        private UserActivityChannel _channel;

        public UserActivitySession GetCurrentActivitySession()
        {
            return _currentUserActivitySession;
        }

        public RoamingService()
        {
            _channel = UserActivityChannel.GetDefault();
        }

        public async Task<UserActivityObject> GetActivityAsync(string id)
        {
            return (await SoundByteService.Current.GetAsync<UserActivityObject>(ServiceType.SoundByte, "remote-subsystem/" + id)).Response;
        }

        public async Task<UserActivity> UpdateActivityAsync(ISource<BaseTrack> source, BaseTrack track, IEnumerable<BaseTrack> playlist, string token)
        {           
            var activity = await _channel.GetOrCreateUserActivityAsync("SoundByte.Playback");
            activity.FallbackUri = new Uri("https://soundbytemedia.com/pages/remote-subsystem");

            var continueText = $"Continue listening to {track.Title} by {track.User.Username}";
            activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson("{\"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\",\"type\":\"AdaptiveCard\",\"backgroundImage\":\"" + track.ArtworkUrl + "\",\"version\": \"1.0\",\"body\":[{\"type\":\"Container\",\"items\":[{\"type\":\"TextBlock\",\"text\":\"Now Playing\",\"weight\":\"bolder\",\"size\":\"large\",\"wrap\":true,\"maxLines\":3},{\"type\":\"TextBlock\",\"text\":\"" + continueText + ".\",\"size\": \"default\",\"wrap\": true,\"maxLines\": 3}]}]}");

            // Tell the server to update the track
            try
            {
                var result = await SoundByteService.Current.PostItemAsync(ServiceType.SoundByte, "remote-subsystem",
                    new UserActivityObject
                    {
                        DeviceId = SettingsService.Instance.AppId,
                        CurrentTrack = new TrackServicePair
                        {
                            Service = track.ServiceType,
                            TrackId = track.TrackId
                        },
                        PlaylistToken = token,
                        SourceName = source.GetType().Name,
                        Tracks = playlist.Select(x => new TrackServicePair { Service  = x.ServiceType, TrackId = x.TrackId })
                    });

                // Set the activation url
                activity.ActivationUri = new Uri($"soundbyte://remote/remote-subsystem?id={result.Response.ActivityId}");
            }
            catch (Exception ex)
            {
                activity.ActivationUri = new Uri($"soundbyte://remote/remote-subsystem-fail?reason={ex.Message}");
            }

            await activity.SaveAsync();

            return activity;
        }

        public async Task StartActivityAsync(ISource<BaseTrack> source, BaseTrack track, IEnumerable<BaseTrack> playlist, string token) 
        {
            var activity = await UpdateActivityAsync(source, track, playlist, token);

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                _currentUserActivitySession = activity.CreateSession();
            });
        }

        public class TrackServicePair
        {
            public string TrackId { get; set; }
            public ServiceType Service { get; set; }
        }


        public class UserActivityObject
        {
            [JsonProperty("tracks")]
            public IEnumerable<TrackServicePair> Tracks { get; set; }

            [JsonProperty("track")]
            public TrackServicePair CurrentTrack { get; set; }

            [JsonProperty("playlist_token")]
            public string PlaylistToken { get; set; }

            [JsonProperty("device_id")]
            public string DeviceId { get; set; }

            [JsonProperty("activity_id")]
            public string ActivityId { get; set; }

            [JsonProperty("source")]
            public string SourceName { get; set; }
        }

        public async Task StopActivityAsync(TimeSpan? currentPosition = null)
        {
            // Set the current position (in case of app close)
            if (currentPosition.HasValue)
            {
                var activity = await _channel.GetOrCreateUserActivityAsync("SoundByte.Playback");
                activity.ActivationUri = new Uri(activity.ActivationUri + "&timespan=" + currentPosition.Value.TotalMilliseconds);
                await activity.SaveAsync();
            }


            if (_currentUserActivitySession != null)
                _currentUserActivitySession.Dispose();
        }

    }
}
