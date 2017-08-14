using System;
using System.Collections.Generic;
using System.Text;

namespace SoundByte.Aurora.Providers.AuthenticationProviders
{
    public class SoundCloudAuthenticationProvider : AuthenticationProvider
    {


        public SoundCloudAuthenticationProvider(string test = null) : base(test)
        {
            test = "hello world";
        }
    }
}
