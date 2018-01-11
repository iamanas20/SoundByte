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
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.YouTube
{
    [UsedImplicitly]
    public class YouTubeLikeSource : ISource<BaseTrack>
    {
        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // If the user has not connected their YouTube account.
            if (!SoundByteService.Current.IsServiceConnected(ServiceType.YouTube))
            {
                return await Task.Run(() =>
                    new SourceResponse<BaseTrack>(null, null, false,
                        Resources.Resources.Sources_YouTube_NoAccount_Title,
                        Resources.Resources.Sources_YouTube_Like_NoAccount_Description));
            }

            return await Task.Run(() =>
                new SourceResponse<BaseTrack>(null, null, false,
                    "Under Development",
                    "This is still under development"));
        }
    }
}
