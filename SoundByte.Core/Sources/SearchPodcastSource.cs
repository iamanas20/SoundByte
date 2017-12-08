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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items;

namespace SoundByte.Core.Sources
{
    [UsedImplicitly]
    public class SearchPodcastSource : ISource<PodcastShow>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<PodcastShow>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Search for podcasts
            var podcasts = await PodcastShow.SearchAsync(SearchQuery);

            // If there are no podcasts
            if (!podcasts.Any())
            {
                return new SourceResponse<PodcastShow>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // return the items
            return new SourceResponse<PodcastShow>(podcasts, "eol");
        }
    }
}
