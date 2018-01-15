using System;

namespace SoundByte.Core.Exceptions
{
    /// <summary>
    ///     Thrown when trying to access a service which does not exist
    /// </summary>
    [Serializable]
    public class ServiceNotExistException : Exception
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ServiceType ServiceType { get; set; }

        public ServiceNotExistException(ServiceType service) : base("An error occured while trying to access the following service, it does not exist: " + service)
        {
            ServiceType = service;
        }
    }
}