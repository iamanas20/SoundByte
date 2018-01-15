using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundByte
{
    [UsedImplicitly]
    public class SoundByteLikeSource : ISource<BaseTrack>
    {
        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // If the user has not connected their SoundByte account.
            if (!SoundByteService.Current.IsSoundByteAccountConnected)
            {
                return await Task.Run(() =>
                    new SourceResponse<BaseTrack>(null, null, false,
                        Resources.Resources.Sources_SoundByte_NoAccount_Title,
                        Resources.Resources.Sources_SoundByte_Like_NoAccount_Description));
            }

            return await Task.Run(() =>
                new SourceResponse<BaseTrack>(null, null, false,
                    "Under Development",
                    "This is still under development"));
        }
    }
}
