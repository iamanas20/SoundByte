using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Track;

namespace SoundByte.Core.Sources.Fanburst.User
{
    [UsedImplicitly]
    public class FanburstUserTrackSource : ISource<BaseTrack>
    {
        public Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            throw new NotImplementedException();
        }
    }
}
