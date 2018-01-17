using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.Fanburst.Search
{
    [UsedImplicitly]
    public class FanburstSearchTrackSource : ISource<BaseTrack>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the Fanburst API and get the items
            var tracks = await SoundByteService.Current.GetAsync<List<FanburstTrack>>(ServiceType.Fanburst, "tracks/search",
                new Dictionary<string, string>
                {
                    {"query", WebUtility.UrlEncode(SearchQuery)},
                    { "page", token },
                    {"per_page", count.ToString()}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // Convert Fanburst specific tracks to base tracks
            var baseTracks = new List<BaseTrack>();
            tracks.Response.ForEach(x => baseTracks.Add(x.ToBaseTrack()));

            tracks.Headers.TryGetValues("X-Next-Page", out var nextPage);
            token = nextPage.FirstOrDefault();

            if (string.IsNullOrEmpty(token))
                token = "eol";

            // Return the items
            return new SourceResponse<BaseTrack>(baseTracks, token);
        }
    }
}
