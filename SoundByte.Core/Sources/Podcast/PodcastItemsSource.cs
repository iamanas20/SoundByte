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
        public PodcastShow Show { get; set; }

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
