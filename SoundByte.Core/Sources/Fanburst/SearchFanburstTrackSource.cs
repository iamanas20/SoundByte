/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.Fanburst
{
    [UsedImplicitly]
    public class SearchFanburstTrackSource : ISource<BaseTrack>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the Fanburst API and get the items
            var tracks = await SoundByteV3Service.Current.GetAsync<List<FanburstTrack>>(ServiceType.Fanburst, "tracks/search",
                new Dictionary<string, string>
                {
                    {"query", WebUtility.UrlEncode(SearchQuery)},
                    {"per_page", count.ToString()}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // Convert Fanburst specific tracks to base tracks
            var baseTracks = new List<BaseTrack>();
            tracks.ForEach(x => baseTracks.Add(x.ToBaseTrack()));

            // Return the items
            return new SourceResponse<BaseTrack>(baseTracks, "eol");
        }
    }
}
