﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.Fanburst
{
    [UsedImplicitly]
    public class FanburstPopularTracksSource : ISource<BaseTrack>
    {
        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>();
        }

        public void ApplyParameters(Dictionary<string, object> data)
        {
            // Not used
        }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the Fanburst API and get the items
            var tracks = await SoundByteService.Current.GetAsync<List<FanburstTrack>>(ServiceType.Fanburst, "tracks/trending", null, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "No results found", "There are no popular Fanburst items.");
            }

            // Convert Fanburst specific tracks to base tracks
            var baseTracks = new List<BaseTrack>();
            tracks.Response.ForEach(x => baseTracks.Add(x.ToBaseTrack()));

            // Return the items
            return new SourceResponse<BaseTrack>(baseTracks, "eol");
        }
    }
}
