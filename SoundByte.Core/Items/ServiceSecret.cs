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

using System.Diagnostics.CodeAnalysis;

namespace SoundByte.Core.Items
{
    /// <summary>
    /// Used to store information about a service for the new Core system.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ServiceSecret
    {
        /// <summary>
        /// The service that this client id - secret pair belong to
        /// </summary>
        public ServiceType Service { get; set; }

        /// <summary>
        /// Client ID used to access API resources
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client secret used to access private resources
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The logged in users token
        /// </summary>
        public LoginToken UserToken { get; set; }
    }
}
