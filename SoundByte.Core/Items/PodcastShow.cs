/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using Newtonsoft.Json;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SoundByte.Core.Items
{
    [JsonObject]
    public class PodcastShow
    {
        [JsonProperty("trackId")]
        public int Id { get; set; }

        [JsonProperty("artistName")]
        public string Username { get; set; }

        [JsonProperty("trackName")]
        public string Title { get; set; }

        [JsonProperty("feedUrl")]
        public string FeedUrl { get; set; }

        [JsonProperty("artworkUrl600")]
        public string ArtworkUrl { get; set; }

        [JsonProperty("trackCount")]
        public int TrackCount { get; set; }

        [JsonProperty("releaseDate")]
        public DateTime Created { get; set; }

        [JsonProperty("genres")]
        public List<string> Genres { get; set; }

        [JsonObject]
        private class Root
        {
            [JsonProperty("results")]
            public List<PodcastShow> Items { get; set; }
        }

        public async Task<List<PodcastEpisode>> GetEpisodesAsync()
        {
            try
            {
                return await Task.Run(async () => {
                    using (var client = new HttpClient(new HttpClientHandler
                    {
                        AutomaticDecompression =
                        System.Net.DecompressionMethods.GZip |
                        System.Net.DecompressionMethods.Deflate
                    }))
                    {
                        using (var request = await client.GetAsync(FeedUrl))
                        {
                            request.EnsureSuccessStatusCode();

                            using (var stream = await request.Content.ReadAsStreamAsync())
                            {
                                var xmlDocument = XDocument.Load(stream);

                                return xmlDocument.Root.Element("channel").Elements("item")
                                    .Select(x => new PodcastEpisode
                                    {
                                        Title = x.Element("title").Value
                                    }).ToList();
                            }
                        }
                    }
                });
            }
            catch (OperationCanceledException)
            {
                return new List<PodcastEpisode>();
            }
            catch (HttpRequestException)
            {
                throw new SoundByteException("No connection?", "Could not get any results, make sure you are connected to the internet.");
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Something went wrong", ex.Message);
            }
        }

        public static async Task<List<PodcastShow>> SearchAsync(string searchTerms)
        {
            return (await SoundByteV3Service.Current.GetAsync<Root>(ServiceType.ITunesPodcast, "/search", new Dictionary<string, string> {
                { "term", searchTerms },
                { "country", "us" },
                { "entity", "podcast" }
            })).Items;
        }
    }
}
