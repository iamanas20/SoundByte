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

using System;
using Newtonsoft.Json;

namespace SoundByte.API.Endpoints
{
    [JsonObject]
    [Obsolete]
    public class LoginInfo
    {
        public ServiceType ServiceType { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string ExpireTime { get; set; }
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
        public string LoginCode { get; set; }
    }
}
