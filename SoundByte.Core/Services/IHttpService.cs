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

using System.Net.Http;
using System.Threading.Tasks;

namespace SoundByte.Core.Services
{
    /// <summary>
    /// Performs HTTP requests
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Performs a generic HTTP request
        /// </summary>
        Task<HttpResponseMessage> PerformRequestAsync(HttpRequestMessage request);
    }
}
