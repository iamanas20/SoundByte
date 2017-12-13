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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Items
{
    /// <summary>
    /// Used to store information about a service for the new Core system.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ServiceInfo
    {
        /// <summary>
        /// What service this info belongs to
        /// </summary>
        public ServiceType Service { get; set; }

        /// <summary>
        /// Client ID used to access API resources
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// A list of backup client Ids
        /// </summary>
        public IEnumerable<string> ClientIds { get; set; }

        /// <summary>
        /// The logged in users token
        /// </summary>
        [CanBeNull]
        public LoginToken UserToken { get; set; }

        /// <summary>
        /// The current logged in user. This can be null if no user is logged in
        /// with this account
        /// </summary>
        [CanBeNull]
        public BaseUser CurrentUser { get; set; }
    }
}
