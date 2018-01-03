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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundCloud.Search
{
    [UsedImplicitly]
    public class SoundCloudSearchPlaylistSource : ISource<BasePlaylist>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<BasePlaylist>> GetItemsAsync(int count, string token, CancellationTokenSource cancellationToken = null)
        {
            // Call the SoundCloud API and get the items
            var playlists = await SoundByteService.Current.GetAsync<SearchPlaylistHolder>(ServiceType.SoundCloud, "/playlists",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"offset", token},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no playlists
            if (!playlists.Playlists.Any())
            {
                return new SourceResponse<BasePlaylist>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // Parse uri for offset
            var param = new QueryParameterCollection(playlists.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific playlists to base playlists
            var basePlaylists = new List<BasePlaylist>();
            playlists.Playlists.ForEach(x => basePlaylists.Add(x.ToBasePlaylist()));

            // Return the items
            return new SourceResponse<BasePlaylist>(basePlaylists, nextToken);
        }

        [JsonObject]
        private class SearchPlaylistHolder
        {
            /// <summary>
            ///     List of playlists
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudPlaylist> Playlists { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}
