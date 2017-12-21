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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.YouTube
{
    [UsedImplicitly]
    public class SearchYouTubePlaylistSource : ISource<BasePlaylist>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<BasePlaylist>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the YouTube API and get the items
            var playlists = await SoundByteV3Service.Current.GetAsync<YouTubeSearchHolder>(ServiceType.YouTube, "search",
                new Dictionary<string, string>
                {
                    {"part", "id"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"type", "playlist"},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!playlists.Playlists.Any())
            {
                return new SourceResponse<BasePlaylist>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // We now need to get the content details (ugh)
            var youTubeIdList = string.Join(",", playlists.Playlists.Select(m => m.Id.PlaylistId));

            var extendedPlaylists = await SoundByteV3Service.Current.GetAsync<YouTubeSearchHolder>(ServiceType.YouTube, "playlists",
                new Dictionary<string, string>
                {
                    {"part", "snippet,contentDetails"},
                    { "id", youTubeIdList }
                }, cancellationToken).ConfigureAwait(false);


            // Convert YouTube specific tracks to base tracks
            var basePlaylists = new List<BasePlaylist>();
            foreach (var playlist in extendedPlaylists.Playlists)
            {
                if (playlist.Id.Kind == "youtube#playlist")
                {
                    basePlaylists.Add(playlist.ToBasePlaylist());
                }
            }

            // Return the items
            return new SourceResponse<BasePlaylist>(basePlaylists, playlists.NextList);
        }

        [JsonObject]
        private class YouTubeSearchHolder
        {
            /// <summary>
            ///     Collection of playlists
            /// </summary>
            [JsonProperty("items")]
            public List<YouTubePlaylist> Playlists { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("nextPageToken")]
            public string NextList { get; set; }
        }
    }
}