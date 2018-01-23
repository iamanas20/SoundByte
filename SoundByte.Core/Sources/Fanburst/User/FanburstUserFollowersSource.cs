using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Sources.Fanburst.User
{
    [UsedImplicitly]
    public class FanburstUserFollowersSource : ISource<BaseUser>
    {
        //https://api.fanburst.com/me/followers

        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>();
        }

        public void ApplyParameters(Dictionary<string, object> data)
        {
            // Not used
        }

        public Task<SourceResponse<BaseUser>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            throw new NotImplementedException();
        }
    }
}
