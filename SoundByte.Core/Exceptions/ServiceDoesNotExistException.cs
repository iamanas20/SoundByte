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

using System;

namespace SoundByte.Core.Exceptions
{
    /// <summary>
    ///     Thrown when trying to access a service which does not exist
    /// </summary>
    [Serializable]
    public class ServiceDoesNotExistException : Exception
    {
        public ServiceType ServiceType { get; set; }

        public ServiceDoesNotExistException(ServiceType service) : base("An error occured while trying to access the following service, it does not exist: " + service)
        {
            ServiceType = service;
        }
    }
}
