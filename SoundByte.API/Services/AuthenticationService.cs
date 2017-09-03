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
using System.Text;
using SoundByte.API.Providers;

namespace SoundByte.API.Services
{
    public class AuthenticationService
    {
        private static readonly Lazy<AuthenticationService> InstanceHolder =
            new Lazy<AuthenticationService>(() => new AuthenticationService());

        public static AuthenticationService Current => InstanceHolder.Value;

        public string BuildLoginString(Func<IAuthenticationProvider> provider)
        {
            var prov = provider.Invoke();

            prov.ServiceType();

            return string.Empty;
        }

    }
}
