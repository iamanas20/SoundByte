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

namespace SoundByte.API.Items
{
    [JsonObject]
    public class LoginToken
    {
        /// <summary>
        /// Used for remote login
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        ///     The access token
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        ///     The type of token
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        ///     Time for this current token to expire
        /// </summary>
        public string ExpireTime { get; set; }

        /// <summary>
        ///     Token used to refresh
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        ///     What scope we have
        /// </summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Used for remote login
        /// </summary>
        public string LoginCode { get; set; }
    }
}
