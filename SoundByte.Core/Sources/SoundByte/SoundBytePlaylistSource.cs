using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundByte
{
    [UsedImplicitly]
    public class SoundBytePlaylistSource : ISource<BasePlaylist>
    {
        public async Task<SourceResponse<BasePlaylist>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // If the user has not connected their SoundByte account.
            if (!SoundByteService.Current.IsSoundByteAccountConnected)
            {
                return await Task.Run(() =>
                    new SourceResponse<BasePlaylist>(null, null, false,
                        Resources.Resources.Sources_SoundByte_NoAccount_Title,
                        Resources.Resources.Sources_SoundByte_Playlist_NoAccount_Description));
            }

            return await Task.Run(() =>
                new SourceResponse<BasePlaylist>(null, null, false,
                    "Under Development",
                    "This is still under development"));
        }
    }
}