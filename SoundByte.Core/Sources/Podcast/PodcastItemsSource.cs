using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.Podcast
{
    [UsedImplicitly]
    public class PodcastItemsSource : ISource<BaseTrack>
    {
        /// <summary>
        ///     Podcast show source
        /// </summary>
        public BasePodcast Show { get; set; }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token, CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            if (cancellationToken == null)
                cancellationToken = new CancellationTokenSource();

            if (Show == null)
                throw new SoundByteException("Not Loaded", "Items not loaded yet.");

            try
            {
                using (var request = await HttpService.Instance.Client.GetAsync(Show.FeedUrl, cancellationToken.Token).ConfigureAwait(false))
                {
                    request.EnsureSuccessStatusCode();

                    using (var stream = await request.Content.ReadAsStreamAsync())
                    {
                        var xmlDocument = XDocument.Load(stream);

                        return new SourceResponse<BaseTrack>(xmlDocument.Root?.Element("channel")?.Elements("item")
                            .Select(x => new BaseTrack
                            {
                                Title = x.Element("title")?.Value
                            }), "eol");
                    }
                }
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("No connection?", hex.Message + "\n" + Show.FeedUrl);
            }
        }
    }
}
