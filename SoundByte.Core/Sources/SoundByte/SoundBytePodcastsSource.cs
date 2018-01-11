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
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundByte
{
    public class SoundBytePodcastsSource : ISource<PodcastShow>
    {
        [UsedImplicitly]
        public async Task<SourceResponse<PodcastShow>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // If the user has not connected their SoundByte account.
            if (!SoundByteService.Current.IsSoundByteAccountConnected)
            {
                return await Task.Run(() =>
                    new SourceResponse<PodcastShow>(null, null, false,
                        Resources.Resources.Sources_SoundByte_NoAccount_Title,
                        Resources.Resources.Sources_SoundByte_Podcast_NoAccount_Description));
            }

            return await Task.Run(() =>
                new SourceResponse<PodcastShow>(null, null, false,
                    "Under Development",
                    "This is still under development"));
        }
    }
}
