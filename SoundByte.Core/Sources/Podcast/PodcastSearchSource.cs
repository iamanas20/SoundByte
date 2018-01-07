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
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.Podcast
{
    [UsedImplicitly]
    public class PodcastSearchSource : ISource<PodcastShow>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<PodcastShow>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Search for podcasts
            var podcasts = await SoundByteService.Current.GetAsync<Root>(ServiceType.ITunesPodcast, "/search", new Dictionary<string, string> {
                { "term", SearchQuery },
                { "country", "us" },
                { "entity", "podcast" }
            }).ConfigureAwait(false);

            // If there are no podcasts
            if (!podcasts.Items.Any())
            {
                return new SourceResponse<PodcastShow>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // return the items
            return new SourceResponse<PodcastShow>(podcasts.Items, "eol");
        }

        [JsonObject]
        private class Root
        {
            [JsonProperty("results")]
            public List<PodcastShow> Items { get; set; }
        }
    }
}
