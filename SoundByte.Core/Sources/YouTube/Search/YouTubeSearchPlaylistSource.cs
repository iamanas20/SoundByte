using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.YouTube.Search
{
    [UsedImplicitly]
    public class YouTubeSearchPlaylistSource : ISource<BasePlaylist>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<BasePlaylist>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the YouTube API and get the items
            var playlists = await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(ServiceType.YouTube,
                "search",
                new Dictionary<string, string>
                {
                    {"part", "id"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"type", "playlist"},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no playlists
            if (!playlists.Response.Playlists.Any())
            {
                return new SourceResponse<BasePlaylist>(null, null, false, "No results found",
                    "Could not find any results for '" + SearchQuery + "'");
            }

            // We now need to get the content details (ugh)
            var youTubeIdList = string.Join(",", playlists.Response.Playlists.Select(m => m.Id.PlaylistId));

            var extendedPlaylists = await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(ServiceType.YouTube,
                "playlists",
                new Dictionary<string, string>
                {
                    {"part", "snippet,contentDetails"},
                    {"id", youTubeIdList}
                }, cancellationToken).ConfigureAwait(false);


            // Convert YouTube specific playlists to base playlists
            var basePlaylists = new List<BasePlaylist>();
            foreach (var playlist in extendedPlaylists.Response.Playlists)
            {
                if (playlist.Id.Kind == "youtube#playlist")
                {
                    basePlaylists.Add(playlist.ToBasePlaylist());
                }
            }

            // Return the items
            return new SourceResponse<BasePlaylist>(basePlaylists, playlists.Response.NextList);
        }
    }
}