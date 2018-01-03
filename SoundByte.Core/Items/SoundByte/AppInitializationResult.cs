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

using Newtonsoft.Json;

namespace SoundByte.Core.Items.SoundByte
{
    /// <summary>
    ///     Data returned from the SoundByte app initialization event
    /// </summary>
    [JsonObject]
    public class AppInitializationResult
    {
        /// <summary>
        ///     If the server approves this connection (99.99% always true).
        /// </summary>
        [JsonProperty("success")]
        public bool Successful { get; set; }

        /// <summary>
        ///     If the server rejected this connection, 
        ///     the title for the error.
        /// </summary>
        [JsonProperty("error_title")]
        public string ErrorTitle { get; set; }

        /// <summary>
        ///     If the server rejected this connection, 
        ///     the description for the error.
        /// </summary>
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     Unique ID for this app.
        /// </summary>
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        /// <summary>
        ///     API keys used by SoundByte.
        /// </summary>
        [JsonProperty("app_keys")]
        public AppInitializationKeys AppKeys { get; set; }
    }
}