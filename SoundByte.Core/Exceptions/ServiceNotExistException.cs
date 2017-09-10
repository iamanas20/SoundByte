using System;
using System.Collections.Generic;
using System.Text;

namespace SoundByte.Core.Exceptions
{
    /// <summary>
    /// Thrown when trying to access a service which does not exist
    /// </summary>
    [Serializable]
    public class ServiceNotExistException : Exception
    {
        public ServiceType ServiceType { get; set; }

        public ServiceNotExistException(ServiceType service) : base("The following service does not exist: " + service)
        {
            ServiceType = service;
        }
    }
}
