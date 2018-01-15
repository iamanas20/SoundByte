using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Playlist;

namespace SoundByte.Core.Sources.Fanburst.Search
{
    [UsedImplicitly]
    public class FanburstSearchPlaylistSource : ISource<BasePlaylist>
    {
        public Task<SourceResponse<BasePlaylist>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            throw new System.NotImplementedException();
        }
    }
}