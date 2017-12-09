using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    /// A set of helpers used for general app auth
    /// </summary>
    public static class AuthorizationHelpers
    {
        /// <summary>
        /// Provide an auth code and a service name. This method calls the SoundByte
        /// Website and performs login logic to get the auth token used in app. 
        /// </summary>
        /// <param name="service">The service that this code belongs to.</param>
        /// <param name="authCode">The code you got from the login call</param>
        /// <returns></returns>
        public static async Task<LoginToken> GetAuthTokenAsync(string service, string authCode)
        {
            // Create a http client to get the token
            using (var httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            }))
            {
                // Set the user agent string
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte", "1.0.0"));

                // Encode the body content
                var encodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "service", service.ToLower() },
                    { "code", authCode }
                });

                try
                {
                    // Post to the the respected API
                    using (var postQuery = await httpClient.PostAsync(
                        "https://soundbyte.gridentertainment.net/api/v1/app/auth",
                        encodedContent))
                    {
                        // Ensure successful
                        postQuery.EnsureSuccessStatusCode();

                        // Get the stream
                        using (var stream = await postQuery.Content.ReadAsStreamAsync())
                        {
                            // Read the stream
                            using (var streamReader = new StreamReader(stream))
                            {
                                // Get the text from the stream
                                using (var textReader = new JsonTextReader(streamReader))
                                {
                                    // Used to get the data from JSON
                                    var serializer = new JsonSerializer
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    };

                                    // Get the class from the json
                                    var response = serializer.Deserialize<SoundByteAuthHolder>(textReader);

                                    if (!response.IsSuccess)
                                    {
                                        throw new SoundByteException("Error Logging In", response.ErrorMessage);
                                    }

                                    return response.Token;
                                }
                            }
                        }
                    }
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
        }


        public static async Task<LoginToken> GetNewAuthTokenAsync(string service, string refreshToken)
        {
            // Create a http client to get the token
            using (var httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            }))
            {
                // Set the user agent string
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte", "1.0.0"));

                // Encode the body content
                var encodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "service", service.ToLower() },
                    { "refresh_token", refreshToken }
                });

                try
                {
                    // Post to the the respected API
                    using (var postQuery = await httpClient.PostAsync(
                        "https://soundbyte.gridentertainment.net/api/v1/app/refresh-auth", encodedContent))
                    {
                        // Ensure successful
                        postQuery.EnsureSuccessStatusCode();

                        // Get the stream
                        using (var stream = await postQuery.Content.ReadAsStreamAsync())
                        {
                            // Read the stream
                            using (var streamReader = new StreamReader(stream))
                            {
                                // Get the text from the stream
                                using (var textReader = new JsonTextReader(streamReader))
                                {
                                    // Used to get the data from JSON
                                    var serializer = new JsonSerializer
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    };

                                    // Get the class from the json
                                    var response = serializer.Deserialize<SoundByteAuthHolder>(textReader);

                                    if (!response.IsSuccess)
                                    {
                                        throw new SoundByteException("Error Refreshing Token", response.ErrorMessage);
                                    }

                                    return response.Token;
                                }
                            }
                        }
                    }
                }
                catch (HttpRequestException hex)
                {
                    throw new SoundByteException("SoundByte Server Error", "There is currently an error with the SoundByte services. Please try again later. Message: " + hex.Message);
                }
                catch (Exception ex)
                {
                    throw new SoundByteException("Error Refreshing Token", ex.Message);
                }

            }
        }
    }

}
