using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Sources.Fanburst.User
{
    [UsedImplicitly]
    public class FanburstUserFollowingsSource : ISource<BaseUser>
    {
        public Task<SourceResponse<BaseUser>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            throw new NotImplementedException();
        }
    }
}