using System;
using System.Collections.Generic;
using System.Text;

namespace SoundByte.API.Providers
{
    public class SoundCloudAuthenticationProvider : IAuthenticationProvider
    {
        public ServiceType ServiceType() => API.ServiceType.SoundCloud;
 

        public string Test()
        {
            return "SoundCloud";
        }
    }
}
