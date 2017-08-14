using System;
using System.Collections.Generic;
using System.Text;
using SoundByte.Aurora.Providers.AuthenticationProviders;

namespace SoundByte.Aurora.Services
{
    public class AuthenticationService
    {
        private static readonly Lazy<AuthenticationService> InstanceHolder =
            new Lazy<AuthenticationService>(() => new AuthenticationService());

        public static AuthenticationService Instance => InstanceHolder.Value;

        public void Login(AuthenticationProvider provder)
        {
            System.Diagnostics.Debug.WriteLine(provder.Test);
        }
    }
}
