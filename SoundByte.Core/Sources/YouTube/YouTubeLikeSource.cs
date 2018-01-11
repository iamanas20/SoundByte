using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SoundByte.Core.Items.Track;

namespace SoundByte.Core.Sources.YouTube
{
    public class YouTubeLikeSource : ISource<BaseTrack>
    {
        public Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            throw new NotImplementedException();
        }
    }
}
