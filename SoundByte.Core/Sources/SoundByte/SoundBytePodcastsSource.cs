using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundByte
{
    public class SoundBytePodcastsSource : ISource<BasePodcast>
    {
        [UsedImplicitly]
        public async Task<SourceResponse<BasePodcast>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // If the user has not connected their SoundByte account.
            if (!SoundByteService.Current.IsSoundByteAccountConnected)
            {
                return await Task.Run(() =>
                    new SourceResponse<BasePodcast>(null, null, false,
                        Resources.Resources.Sources_SoundByte_NoAccount_Title,
                        Resources.Resources.Sources_SoundByte_Podcast_NoAccount_Description));
            }

            return await Task.Run(() =>
                new SourceResponse<BasePodcast>(null, null, false,
                    "Under Development",
                    "This is still under development"));
        }
    }
}