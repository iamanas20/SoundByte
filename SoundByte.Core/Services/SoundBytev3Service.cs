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
using System.Collections.Generic;
using System.Linq;
using SoundByte.Core.Items;

namespace SoundByte.Core.Services
{
    public class SoundByteV3Service
    {
        #region Private Variables
        // Has this class performed basic load yet (using Init();)
        private bool _isLoaded;

        private List<ServiceSecret> ServiceSecrets { get; } = new List<ServiceSecret>();

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secrets"></param>
        public void Init(List<ServiceSecret> secrets)
        {
            // A list of secrets must be provided
            if (!secrets.Any())
                throw new Exception("No Keys Provided");

            // Empty any other secrets
            ServiceSecrets.Clear();

            // Loop through all the keys and add them
            foreach (var secret in secrets)
            {
                // If there is already a service in the list, thow an exception, there
                // should only be one key for each service.
                if (ServiceSecrets.FirstOrDefault(x => x.Service == secret.Service) != null)
                    throw new Exception("Only one key for each service!");

                ServiceSecrets.Add(secret);
            }

            _isLoaded = true;
        }

        public void ConnectService(ServiceType type, LoginToken token)
        {
            var serviceSecret = ServiceSecrets.FirstOrDefault(x => x.Service == type);

            if (serviceSecret == null)
                throw new Exception("Service does not exist.");

            // Set the token
            serviceSecret.UserToken = token;
        }

        public void DisconnectService(ServiceType type)
        {
            
        }

        #region Instance Setup
        private static readonly Lazy<SoundByteV3Service> InstanceHolder =
            new Lazy<SoundByteV3Service>(() => new SoundByteV3Service());

        public static SoundByteV3Service Current => InstanceHolder.Value;
        #endregion
    }
}
