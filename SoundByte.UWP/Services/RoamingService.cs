using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Shell;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Track;
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

        public RoamingService()
        {
            _channel = UserActivityChannel.GetDefault();
        }

        public UserActivityObject DecodeActivityParameters(string compressedData)
        {
            // Get the raw data string
            var data = StringHelpers.DecompressString(Uri.UnescapeDataString(compressedData));

            // Get from url
            var paramCollection = data.Split('&');

            // Get raw objects
            var currentTrack = paramCollection.FirstOrDefault(x => x.Split('=')[0] == "c")?.Split('=')[1];
            var sourceName = paramCollection.FirstOrDefault(x => x.Split('=')[0] == "s")?.Split('=')[1];
            var playlistToken = paramCollection.FirstOrDefault(x => x.Split('=')[0] == "t")?.Split('=')[1];
            var tracksRaw = paramCollection.FirstOrDefault(x => x.Split('=')[0] == "p")?.Split('=')[1];

            // Parse current track
            var currentTrackService = (ServiceType) int.Parse(currentTrack.Split('-')[0]);
            var currentTrackId = currentTrack.Split('-')[1];

            // Pass playlist of tracks
            var tracks = new List<TrackServicePair>();

            foreach (var trackPair in tracksRaw.Split(','))
            {
                var trackService = (ServiceType)int.Parse(trackPair.Split('-')[0]);
                var trackId = trackPair.Split('-')[1];

                tracks.Add(new TrackServicePair
                {
                    TrackId = trackId,
                    Service = trackService
                });
            }

            return new UserActivityObject
            {
                Tracks = tracks,
                CurrentTrack = new TrackServicePair
                {
                    TrackId = currentTrackId,
                    Service = currentTrackService
                },
                PlaylistToken = playlistToken,
                SourceName = sourceName
            };

        }

        public string EncodeActivityParameters(ISource<BaseTrack> source, BaseTrack track,
            IEnumerable<BaseTrack> playlist, string token)
        {
            // Conver to raw objects
            var currentTrack = $"c={(int)track.ServiceType}-{track.TrackId}";
            var sourceName = $"s={source.GetType().Name}";
            var playlistToken = $"t={token}";
            var tracks = $"p={string.Join(',', playlist.Select(x => $"{(int)x.ServiceType}-{x.TrackId}"))}";

            // Format into url
            var data = $"{currentTrack}&{sourceName}&{playlistToken}&{tracks}";
            return Uri.EscapeDataString(StringHelpers.CompressString(data));
        }

        public async Task<UserActivity> UpdateActivityAsync(ISource<BaseTrack> source, BaseTrack track, IEnumerable<BaseTrack> playlist, string token)
        {           
            var activity = await _channel.GetOrCreateUserActivityAsync("SoundByte.Playback");
            activity.FallbackUri = new Uri("https://soundbytemedia.com/pages/remote-subsystem");

            var continueText = $"Continue listening to {track.Title} by {track.User.Username}";

            activity.VisualElements.DisplayText = "Now Playing";
            activity.VisualElements.Description = continueText;

            activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson("{\"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\",\"type\":\"AdaptiveCard\",\"backgroundImage\":\"" + track.ArtworkUrl + "\",\"version\": \"1.0\",\"body\":[{\"type\":\"Container\",\"items\":[{\"type\":\"TextBlock\",\"text\":\"Now Playing\",\"weight\":\"bolder\",\"size\":\"large\",\"wrap\":true,\"maxLines\":3},{\"type\":\"TextBlock\",\"text\":\"" + continueText + ".\",\"size\": \"default\",\"wrap\": true,\"maxLines\": 3}]}]}");

            // Set the activation url using shorthand protocol
            activity.ActivationUri = new Uri($"sb://rs?d={EncodeActivityParameters(source, track, playlist, token)}");

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
            public IEnumerable<TrackServicePair> Tracks { get; set; }
            public TrackServicePair CurrentTrack { get; set; }
            public string PlaylistToken { get; set; }
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
