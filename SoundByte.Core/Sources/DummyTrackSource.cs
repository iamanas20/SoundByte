/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

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