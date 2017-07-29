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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace SoundByte.Core.API.Holders
{
    /// <summary>
    /// Holds a list of users
    /// </summary>
    [JsonObject]
    public class UserListHolder
    {
        /// <summary>
        /// List of users
        /// </summary>
        [JsonProperty("collection")]
        public List<Endpoints.User> Users { get; set; }

        /// <summary>
        /// The next list of items
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}
