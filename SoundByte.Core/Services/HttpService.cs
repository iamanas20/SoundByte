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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SoundByte.Core.Services
{
    /// <summary>
    /// A generic HTTP service that uses <see cref="HttpClient"/> for handling requests.
    /// </summary>
    public partial class HttpService : IHttpService, IDisposable
    {
        #region Private Variables
        // Flag: Has Dispose already been called?
        private bool _disposed;

        // Internal instance of <see cref="HttpClient"/>.
        private HttpClient _client;

        // Used to deserialize data.
        private JsonSerializer _jsonSerializer;
        #endregion

        #region Constructors
        /// <summary>
        ///     Creates an instance of <see cref="HttpService"/> with a custom <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client"></param>
        public HttpService(HttpClient client)
        {
            _client = client ?? throw new ArgumentException(nameof(client));

            _jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public HttpService()
        {
            var httpClientHandler = new HttpClientHandler();

            // Handle decompression
            if (httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            // Get version of dll
            var version = typeof(HttpService).GetTypeInfo().Assembly.GetName().Version;

            // Create the http client 
            _client = new HttpClient(httpClientHandler, true);
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte.Core", $"{version.Major}.{version.Minor}.{version.Revision}"));

            _jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }
        #endregion
    

        public Task<T> GetAsync<T>(string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string url, [NotNull] Dictionary<string, string> paramaters)
        {
            // Encode this content so we can send it.
            var encodedContent = new FormUrlEncodedContent(paramaters);

            // Post this request to the url
            using (var postQuery = await _client.PostAsync(url, encodedContent))
            {
                // Ensure this request was successful
                postQuery.EnsureSuccessStatusCode();

                // Get the stream
                using (var stream = await postQuery.Content.ReadAsStreamAsync())
                {
                    // Read the stream
                    using (var streamReader = new StreamReader(stream))
                    {
                        // Get the text from the stream
                        using (var textReader = new JsonTextReader(streamReader))
                        {
                            return _jsonSerializer.Deserialize<T>(textReader);
                        }
                    }
                }
            }
        }




        public void Dispose()
        {
            if (_disposed)
                return;


            _client.Dispose();

            _disposed = true;
        }
    }

    /// <summary>
    /// A generic HTTP service that uses <see cref="HttpClient"/> for handling requests.
    /// </summary>
    public partial class HttpService
    {
        private static HttpService _instance;

        /// <summary>
        /// Singleton instance of <see cref="HttpService"/>.
        /// </summary>
        public static HttpService Instance => _instance ?? (_instance = new HttpService());
    }
}