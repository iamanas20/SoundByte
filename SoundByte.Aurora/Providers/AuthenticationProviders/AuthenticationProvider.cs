using System;
using System.Collections.Generic;
using System.Text;

namespace SoundByte.Aurora.Providers.AuthenticationProviders
{
    public abstract class AuthenticationProvider
    {
        public string Test { get; private set; }

        protected AuthenticationProvider(string test)
        {
            Test = test;
        }
    }
}
