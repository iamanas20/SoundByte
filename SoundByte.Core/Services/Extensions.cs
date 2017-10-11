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


using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SoundByte.Core.Services
{
    /// <summary>
    /// Service extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Send a GET request and return content as a string
        /// </summary>
        public static async Task<string> GetStringAsync(this IHttpService httpService, string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await httpService.PerformRequestAsync(request).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send a GET request and return content as a stream
        /// </summary>
        public static async Task<Stream> GetStreamAsync(this IHttpService httpService, string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                var response = await httpService.PerformRequestAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        }
    }
}
