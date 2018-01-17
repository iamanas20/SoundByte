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
        //https://api.fanburst.com/me/following

        public Task<SourceResponse<BaseUser>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            throw new NotImplementedException();
        }
    }
}