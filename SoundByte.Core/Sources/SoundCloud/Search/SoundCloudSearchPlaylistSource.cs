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

        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "q", SearchQuery }
            };
        }

        public void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("q", out var query);
            SearchQuery = query.ToString();
        }

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
            if (!playlists.Response.Playlists.Any())
            {
                return new SourceResponse<BasePlaylist>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // Parse uri for offset
            var param = new QueryParameterCollection(playlists.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific playlists to base playlists
            var basePlaylists = new List<BasePlaylist>();
            playlists.Response.Playlists.ForEach(x => basePlaylists.Add(x.ToBasePlaylist()));

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
