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
                    { "refreshtoken", refreshToken }
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

        /// <summary>
        /// Init the app with the online service. This is required for the app to work.
        /// </summary>
        /// <param name="platform">What platform this app is running on.</param>
        /// <param name="version">Version of the app</param>
        /// <param name="appId">The Unique app install ID for this app</param>
        /// <param name="requestNewKeys">Tell the server that we want new app keys.</param>
        /// <returns></returns>
        public static async Task<InitResult> OnlineAppInitAsync(string platform, string version, string appId,
            bool requestNewKeys)
        {
            return null;


            // Create a http client to get the token
            using (var httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            }))
            {
                // Set the user agent string
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte", "1.0.0"));

                // Set the user agent string
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte", "1.0.0"));

                // Encode the body content
                var encodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    
                });

                try
                {
                    // Post to the the respected API
                    using (var postQuery = await httpClient.PostAsync(
                        "https://soundbyte.gridentertainment.net/api/v1/app/init", encodedContent))
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
                                    var response = serializer.Deserialize<InitResult>(textReader);

                                    if (!response.Successful)
                                    {
                                        throw new SoundByteException("Error Refreshing Token", response.ErrorMessage);
                                    }

                                    return response;
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
                    throw new SoundByteException("Error Init App", ex.Message);
                }
            }
        }


        [JsonObject]
        public class InitResult
        {
            [JsonProperty("success")]
            public bool Successful { get; set; }

            [JsonProperty("error_title")]
            public string ErrorTitle { get; set; }

            [JsonProperty("error_message")]
            public string ErrorMessage { get; set; }

            [JsonProperty("app_id")]
            public string AppId { get; set; }

            [JsonProperty("app_keys")]
            public AppKeys AppKeys { get; set; }
        }

        public class AppKeys
        {
            public string SoundCloudClientId { get; set; }
            public List<string> SoundCloudPlaybackIds { get; set; }
            public string FanburstClientId { get; set; }
            public string YouTubeClientId { get; set; }
            public string YouTubeLoginClientId { get; set; }
            public string LastFmClientId { get; set; }
            public string HockeyAppClientId { get; set; }
            public string GoogleAnalyticsTrackerId { get; set; }
            public string AppCenterClientId { get; set; }
        }
    }
}
