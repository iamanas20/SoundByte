using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundCloud.User
{
    [UsedImplicitly]
    public class SoundCloudUserTrackSource : ISource<BaseTrack>
    {
        public BaseUser User { get; set; }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the SoundCloud API and get the items
            var tracks = await SoundByteService.Current.GetAsync<TrackHolder>(ServiceType.SoundCloud, $"/users/{User.UserId}/tracks",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"offset", token}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Tracks.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "Nothing to hear here", "Follow " + User.Username + " for updates on sounds they share in the future.");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(tracks.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseTracks = new List<BaseTrack>();
            tracks.Tracks.ForEach(x => baseTracks.Add(x.ToBaseTrack()));

            // Return the items
            return new SourceResponse<BaseTrack>(baseTracks, nextToken);
        }

        [JsonObject]
        private class TrackHolder
        {
            /// <summary>
            ///     Collection of tracks
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudTrack> Tracks { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}
