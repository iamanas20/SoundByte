using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundCloud.Search
{
    /// <summary>
    /// Searches the SoundCloud API for tracks
    /// </summary>
    [UsedImplicitly]
    public class SoundCloudSearchTrackSource : ISource<BaseTrack>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token, 
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the SoundCloud API and get the items
            var tracks = await SoundByteService.Current.GetAsync<TrackListHolder>(ServiceType.SoundCloud, "/tracks",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"offset", token},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Tracks.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // Parse uri for offset
            var param = new QueryParameterCollection(tracks.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseTracks = new List<BaseTrack>();
            tracks.Tracks.ForEach(x => baseTracks.Add(x.ToBaseTrack()));

            // Return the items
            return new SourceResponse<BaseTrack>(baseTracks, nextToken);
        }

        /// <summary>
        /// Private class used to decode SoundCloud tracks
        /// </summary>
        [JsonObject]
        private class TrackListHolder
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
