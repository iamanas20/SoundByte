using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.Fanburst
{
    [UsedImplicitly]
    public class FanburstPlaylistSource : ISource<BasePlaylist>
    {
        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>();
        }

        public void ApplyParameters(Dictionary<string, object> data)
        {
            // Not used
        }

        public async Task<SourceResponse<BasePlaylist>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // If the user has not connected their fanburst account.
            if (!SoundByteService.Current.IsServiceConnected(ServiceType.Fanburst))
            {
                return new SourceResponse<BasePlaylist>(null, null, false,
                        Resources.Resources.Sources_Fanburst_NoAccount_Title,
                        Resources.Resources.Sources_Fanburst_Playlist_NoAccount_Description);
            }

            // Get the users playlists
            var playlists = await SoundByteService.Current.GetAsync<List<FanburstPlaylist>>(ServiceType.Fanburst, "me/playlists", new Dictionary<string, string>
            {
                { "page", token },
                { "per_page", count.ToString() }
            }, cancellationToken).ConfigureAwait(false);

            // If there are no likes
            if (!playlists.Response.Any())
            {
                return new SourceResponse<BasePlaylist>(null, null, false, 
                    Resources.Resources.Sources_All_NoResults_Title, 
                    Resources.Resources.Sources_Fanburst_Playlist_NoItems_Description);
            }

            // Convert Fanburst specific tracks to base tracks
            var basePlaylists = new List<BasePlaylist>();
            playlists.Response.ForEach(x => basePlaylists.Add(x.ToBasePlaylist()));

            playlists.Headers.TryGetValues("X-Next-Page", out var nextPage);
            token = nextPage.FirstOrDefault();

            if (string.IsNullOrEmpty(token))
                token = "eol";

            return new SourceResponse<BasePlaylist>(basePlaylists, token);
        }
    }
}
