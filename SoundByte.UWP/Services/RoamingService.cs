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

        private string GetParameterString(string[] parameters, string key)
        {
            var item = parameters.Where(x => x.Contains("=")).FirstOrDefault(x => x.Split('=')[0] == key);

            if (string.IsNullOrEmpty(item))
                return item;

            return item.Split('=')[1];
        }

        public UserActivityObject DecodeActivityParameters(string compressedData)
        {
            // Get the raw data string
            var data = StringHelpers.DecompressString(Uri.UnescapeDataString(compressedData));

            // Get from url
            var paramCollection = data.Split('&');

            // Get raw objects
            var currentTrack = GetParameterString(paramCollection, "c"); 
            var sourceName = GetParameterString(paramCollection, "s");
            var sourceDetails = GetParameterString(paramCollection, "d");
            var playlistToken = GetParameterString(paramCollection, "t");
            var tracksRaw = GetParameterString(paramCollection, "p");

            // Parse current track
            var currentTrackService = (ServiceType) int.Parse(currentTrack?.Split('-')[0]);
            var currentTrackId = currentTrack?.Split('-')[1];

            // generate the source
            var detailDict = sourceDetails.Split(',').ToDictionary(x => x.Split('-')[0], x => (object)x.Split('-')[1]);
            var sourceRaw = App.SourceManager.GetTrackSource(sourceName, detailDict);

            // Pass playlist of tracks
            var tracks = new List<TrackServicePair>();

            if (tracksRaw != null)
            {
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
                Source = sourceRaw
            };

        }

        public string EncodeActivityParameters(ISource<BaseTrack> source, BaseTrack track,
            IEnumerable<BaseTrack> playlist, string token)
        {
            // Conver to raw objects
            var currentTrack = $"c={(int)track.ServiceType}-{track.TrackId}";
            var sourceName = $"&s={source.GetType().Name}";
            var sourceDetails = $"&d={string.Join(',', source.GetParameters().Select(x => $"{x.Key}-{x.Value}"))}";
            var playlistToken = $"&t={token}";
            var tracks = $"&p={string.Join(',', playlist.Select(x => $"{(int)x.ServiceType}-{x.TrackId}"))}";

            // If we don't have a token, we do not need to send the tracks
            // unless this is a dummy source. This is because our source can 
            // load the first set of items.
            if (string.IsNullOrEmpty(token) && source.GetType() != typeof(DummyTrackSource))
                tracks = "";

            // Format into url
            var data = $"{currentTrack}{sourceName}{sourceDetails}{playlistToken}{tracks}";
            return Uri.EscapeDataString(StringHelpers.CompressString(data));
        }

        public async Task<UserActivity> UpdateActivityAsync(ISource<BaseTrack> source, BaseTrack track, IEnumerable<BaseTrack> playlist, string token)
        {
            // We do not support these items
            if (track.ServiceType == ServiceType.ITunesPodcast ||
                track.ServiceType == ServiceType.Local ||
                track.ServiceType == ServiceType.Unknown)
                return null;

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
            // We do not support these items
            if (track.ServiceType == ServiceType.ITunesPodcast ||
                track.ServiceType == ServiceType.Local ||
                track.ServiceType == ServiceType.Unknown)
                return;

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



            public ISource<BaseTrack> Source { get; set; }
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
