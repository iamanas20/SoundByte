using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Track;

namespace SoundByte.Core.Sources
{
    /// <summary>
    /// Empty source for tracks. This is used when trying to build a SoundByte
    /// collection when using a list of songs and not a model.
    /// </summary>
    [UsedImplicitly]
    public class DummyTrackSource : ISource<BaseTrack>
    {
        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            return await Task.Run(() => new SourceResponse<BaseTrack>(null, null, false));
        }
    }
}