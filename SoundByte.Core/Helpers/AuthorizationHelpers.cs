using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items;
using SoundByte.Core.Items.SoundByte;
using SoundByte.Core.Services;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    ///     These helpers are used for communicating with the SoundByte website.
    /// </summary>
    public static class AuthorizationHelpers
    {
        /// <summary>
        ///     Provide an auth code and a service name. This method calls the SoundByte
        ///     Website and performs login logic to get the auth token used in app. 
        /// </summary>
        /// <param name="service">The service that this code belongs to.</param>
        /// <param name="authCode">The code you got from the login call</param>
        /// <returns></returns>
        public static async Task<LoginToken> GetAuthTokenAsync(ServiceType service, string authCode)
        {
            try
            {
                var result = await HttpService.Instance.PostAsync<SoundByteAuthHolder>("https://soundbytemedia.com/api/v1/app/auth",
                    new Dictionary<string, string>
                    {
                        { "service", service.ToString().ToLower() },
                        { "code", authCode }
                    });

                if (!result.Response.IsSuccess)
                {
                    throw new SoundByteException("Error Logging In", result.Response.ErrorMessage);
                }

                return result.Response.Token;
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("SoundByte Server Error", "There is currently an error with the SoundByte services. Please try again later. Message: " + hex.Message);
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Error Logging In", ex.Message);
            }
        }

        public static async Task<LoginToken> GetNewAuthTokenAsync(ServiceType service, string refreshToken)
        {
            try
            {
                var result = await HttpService.Instance.PostAsync<SoundByteAuthHolder>("https://soundbytemedia.com/api/v1/app/refresh-auth",
                    new Dictionary<string, string>
                    {
                        { "service", service.ToString().ToLower() },
                        { "refreshtoken", refreshToken }
                    });

                if (!result.Response.IsSuccess)
                {
                    throw new SoundByteException("Error Refreshing Token", result.Response.ErrorMessage);
                }

                return result.Response.Token;
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("SoundByte Server Error", "There is currently an error with the SoundByte services. Please try again later. Message: " + hex.Message);
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Error Logging In", ex.Message);
            }
        }

        /// <summary>
        ///     Init the app with the online service. This is required for the app to work.
        /// </summary>
        /// <param name="platform">What platform this app is running on.</param>
        /// <param name="version">Version of the app</param>
        /// <param name="appId">The Unique app install ID for this app</param>
        /// <param name="requestNewKeys">Tell the server that we want new app keys.</param>
        /// <returns></returns>
        public static async Task<AppInitializationResult> OnlineAppInitAsync(string platform, string version, string appId,
            bool requestNewKeys)
        {
            try
            {
                var result = await HttpService.Instance.PostAsync<AppInitializationResult>("https://soundbytemedia.com/api/v1/app/init",
                    new Dictionary<string, string>
                    {
                        { "requestnewkeys", requestNewKeys.ToString() },
                        { "appid", appId },
                        { "platform", platform },
                        { "version", version }
                    });

                if (!result.Response.Successful)
                {
                    throw new SoundByteException(result.Response.ErrorTitle, result.Response.ErrorMessage);
                }

                return result.Response;
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("SoundByte Server Error", "There is currently an error with the SoundByte services. Please try again later. Message: " + hex.Message);
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Error Init App", ex.Message);
            }
        }
    }
}