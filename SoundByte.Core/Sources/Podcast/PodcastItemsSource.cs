using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
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

        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "s", Show.FeedUrl }
            };
        }

        public void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("s", out var show);
            Show = new BasePodcast
            {
                FeedUrl = show.ToString()
            };
        }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token, CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            if (cancellationToken == null)
                cancellationToken = new CancellationTokenSource();

            if (Show == null)
                throw new SoundByteException("Not Loaded", "Items not loaded yet.");

            var tracks = new List<BaseTrack>();

            try
            {
                using (var request = await HttpService.Instance.Client.GetAsync(Show.FeedUrl, cancellationToken.Token).ConfigureAwait(false))
                {
                    request.EnsureSuccessStatusCode();

                    using (var stream = await request.Content.ReadAsStreamAsync())
                    {
                        // Load the document
                        var xmlDocument = XDocument.Load(stream);

                        // Get channel
                        var channel = xmlDocument.Root?.Element("channel");

                        // Get all the feed items
                        var feedItems = channel?.Elements("item");

                        XNamespace ns = "http://www.itunes.com/dtds/podcast-1.0.dtd";

                        foreach (var feedItem in feedItems)
                        {
                            tracks.Add(new BaseTrack
                            {
                                TrackId = feedItem.Element("guid")?.Value,
                                ServiceType = ServiceType.ITunesPodcast,
                                Title = feedItem.Element("title")?.Value,
                           //     Created = DateTime.Parse(feedItem.Element("pubDate")?.Value), //todo later
                                ArtworkUrl = feedItem.Element(ns + "image")?.Value,
                                AudioStreamUrl = feedItem.Element("enclosure").Attribute("url")?.Value,
                            //    Duration = TimeSpan.Parse(feedItem.Element(ns + "duration")?.Value), //todo later
                                User = new BaseUser { Username = channel?.Element(ns + "author")?.Value },
                                Description = feedItem.Element("description")?.Value

                            });

                        }
                    }
                }

                return new SourceResponse<BaseTrack>(tracks, "eol");
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("Error", hex.Message + "\n" + Show.FeedUrl);
            }
        }
    }
}