using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.Fanburst
{
    [UsedImplicitly]
    public class FanburstLikeSource : ISource<BaseTrack>
    {
        //https://api.fanburst.com/me/favorites

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // If the user has not connected their YouTube account.
            if (!SoundByteService.Current.IsServiceConnected(ServiceType.Fanburst))
            {
                return await Task.Run(() =>
                    new SourceResponse<BaseTrack>(null, null, false,
                        Resources.Resources.Sources_Fanburst_NoAccount_Title,
                        Resources.Resources.Sources_Fanburst_Like_NoAccount_Description));
            }

            // Get the users likes
            var likes = await SoundByteService.Current.GetAsync<List<FanburstTrack>>(ServiceType.Fanburst, "me/favorites", new Dictionary<string, string>
            {
                { "page", token },
                { "per_page", count.ToString() }
            }, cancellationToken).ConfigureAwait(false);

            // If there are no likes
            if (!likes.Response.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "No results found", "Like some items to start");
            }

            // Convert Fanburst specific tracks to base tracks
            var baseTracks = new List<BaseTrack>();
            likes.Response.ForEach(x => baseTracks.Add(x.ToBaseTrack()));


            return new SourceResponse<BaseTrack>(baseTracks, null);
        }
    }
}
