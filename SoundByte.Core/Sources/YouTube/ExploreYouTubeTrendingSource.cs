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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.YouTube
{
    [UsedImplicitly]
    public class ExploreYouTubeTrendingSource : ISource<BaseTrack>
    {
        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the YouTube API and get the items
            var tracks = await SoundByteService.Current.GetAsync<YouTubeExploreHolder>(ServiceType.YouTube, "videos",
                new Dictionary<string, string>
                {
                    {"part", "snippet,contentDetails"},
                    {"chart", "mostPopular"},
                    {"maxResults", count.ToString()},
                    {"videoCategoryId", "10"},
                    {"pageToken", token},
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Tracks.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "No results found", "There are no trending YouTube items.");
            }

            // Convert YouTube specific tracks to base tracks
            var baseTracks = new List<BaseTrack>();
            foreach (var track in tracks.Tracks)
            {
                if (track.Id.Kind == "youtube#video")
                {
                    baseTracks.Add(track.ToBaseTrack());
                }
            }

            // Return the items
            return new SourceResponse<BaseTrack>(baseTracks, tracks.NextList);
        }

        [JsonObject]
        private class YouTubeExploreHolder
        {
            [JsonProperty("nextPageToken")]
            public string NextList { get; set; }

            [JsonProperty("items")]
            public List<YouTubeTrack> Tracks { get; set; }
        }
    }
}
