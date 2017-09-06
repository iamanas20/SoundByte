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
    /// <summary>
    ///     Info about the current build
    /// </summary>
    [JsonObject]
    public class BuildInformation
    {
        /// <summary>
        ///     The branch that this was compliled from
        /// </summary>
        [JsonProperty("build_branch")]
        public string BuildBranch { get; set; }

        /// <summary>
        ///     The time this was built
        /// </summary>
        [JsonProperty("build_time")]
        public string BuildTime { get; set; }
    }
}
