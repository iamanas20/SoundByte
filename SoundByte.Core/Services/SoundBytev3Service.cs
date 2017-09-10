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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items;
using System.IO;
using Newtonsoft.Json;

namespace SoundByte.Core.Services
{
    /// <summary>
    /// Next generation (Gen3.0) SoundByte Service. New features include portability (.NET Standard),
    /// events (event based e.g OnServiceConnected), muiltiple services, easy to extend.
    /// </summary>
    public class SoundByteV3Service
    {
        #region Delegates
        public delegate void ServiceConnectedEventHandler(ServiceType type, LoginToken token);
        public delegate void ServiceDisconnectedEventHandler(ServiceType type);
        #endregion

        #region Events
        /// <summary>
        /// This event is fired when a service is connected to SoundByte.
        /// When this event fires you should store the token somewhere within
        /// your application.
        /// </summary>
        public event ServiceConnectedEventHandler OnServiceConnected;

        /// <summary>
        /// This event is fired when a service is disconnected. When this event fires
        /// you should remove any saved tokens and update the appropiate UI.
        /// </summary>
        public event ServiceDisconnectedEventHandler OnServiceDisconnected;
        #endregion

        #region Private Variables
        // Has this class performed basic load yet (using Init();)
        private bool _isLoaded;

        /// <summary>
        /// Private list of services and their client id / client secrets.
        /// Also contains login information.
        /// </summary>
        private List<ServiceSecret> ServiceSecrets { get; } = new List<ServiceSecret>();

        #endregion

        #region Getters and Setters

        #endregion

        #region Instance Setup
        private static readonly Lazy<SoundByteV3Service> InstanceHolder =
            new Lazy<SoundByteV3Service>(() => new SoundByteV3Service());

        /// <summary>
        /// Gets the current instance of SoundByte V3 Service
        /// </summary>
        public static SoundByteV3Service Current => InstanceHolder.Value;
        #endregion

        /// <summary>
        /// Setup the service
        /// </summary>
        /// <param name="secrets">A list of services and their secrets that will be used in the app</param>
        public void Init(List<ServiceSecret> secrets)
        {
            // A list of secrets must be provided
            if (!secrets.Any())
                throw new Exception("No Keys Provided");

            // Empty any other secrets
            ServiceSecrets.Clear();

            // Loop through all the keys and add them
            foreach (var secret in secrets)
            {
                // If there is already a service in the list, thow an exception, there
                // should only be one key for each service.
                if (ServiceSecrets.FirstOrDefault(x => x.Service == secret.Service) != null)
                    throw new Exception("Only one key for each service!");

                ServiceSecrets.Add(secret);
            }

            _isLoaded = true;
        }

        #region Service Methods
        /// <summary>
        /// Connects a service to SoundByte. This will allow accessing
        /// user content. The ServiceConnected event is fired.
        /// </summary>
        /// <param name="type">The service to connect.</param>
        /// <param name="token">The required token</param>
        public void ConnectService(ServiceType type, LoginToken token)
        {
            var serviceSecret = ServiceSecrets.FirstOrDefault(x => x.Service == type);
            if (serviceSecret == null)
                throw new ServiceNotExistException(type);

            // Set the token
            serviceSecret.UserToken = token;

            // Fire the event
            OnServiceConnected?.Invoke(type, token);
        }

        /// <summary>
        /// Disconnects a specified service from SoundByte and
        /// fires the service disconnected event handler.
        /// </summary>
        /// <param name="type">The service to disconnect</param>
        public void DisconnectService(ServiceType type)
        {
            // Get the service information
            var service = ServiceSecrets.FirstOrDefault(x => x.Service == type);
            if (service == null)
                throw new ServiceNotExistException(type);

            // Delete the user token
            service.UserToken = null;

            // Fire the event
            OnServiceDisconnected?.Invoke(type);
        }

        /// <summary>
        /// Is the user logged into a service. Warning: will throw an exception if
        /// the service does not exsit.
        /// </summary>
        /// <param name="type">The service to check if the user has connected.</param>
        /// <returns>If the user accounted is connected</returns>
        public bool IsServiceConnected(ServiceType type)
        {
            // Get the service information
            var service = ServiceSecrets.FirstOrDefault(x => x.Service == type);

            if (service == null)
                throw new ServiceNotExistException(type);

            // If the user token is not null, we are connected
            return service.UserToken != null;
        }
        #endregion

        #region Web API

        /// <summary>
        /// Fetches an object from the specified service API and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="endpoint"></param>
        /// <param name="optionalParams"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(ServiceType type, string endpoint, Dictionary<string, string> optionalParams = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (_isLoaded == false)
                throw new Exception("Not loaded");

            // Strip out the / infront of the endpoint if it exists
            endpoint = endpoint.TrimStart('/');

            // Start building the request URL
            var requestUri = string.Empty;

            switch (type)
            {
                case ServiceType.SoundCloud:
                    var soundCloudService = ServiceSecrets.FirstOrDefault(x => x.Service == ServiceType.SoundCloud);
                    if (soundCloudService == null)
                        throw new ServiceNotExistException(ServiceType.SoundCloud);

                    requestUri = $"https://api.soundcloud.com/{endpoint}?client_id={soundCloudService.ClientId}&client_secret={soundCloudService.ClientSecret}";
                    break;

                case ServiceType.SoundCloudV2:
                    var soundCloudV2Service = ServiceSecrets.FirstOrDefault(x => x.Service == ServiceType.SoundCloudV2);
                    if (soundCloudV2Service == null)
                        throw new ServiceNotExistException(ServiceType.SoundCloudV2);

                    requestUri = $"https://api-v2.soundcloud.com/{endpoint}?client_id={soundCloudV2Service.ClientId}&client_secret={soundCloudV2Service.ClientSecret}";
                    break;

                case ServiceType.Fanburst:
                    var fanburstService = ServiceSecrets.FirstOrDefault(x => x.Service == ServiceType.Fanburst);
                    if (fanburstService == null)
                        throw new ServiceNotExistException(ServiceType.Fanburst);

                    requestUri = $"https://api.fanburst.com/{endpoint}?client_id={fanburstService.ClientId}&client_secret={fanburstService.ClientSecret}";
                    break;
                case ServiceType.YouTube:
                    var youtubeService = ServiceSecrets.FirstOrDefault(x => x.Service == ServiceType.YouTube);
                    if (youtubeService == null)
                        throw new ServiceNotExistException(ServiceType.YouTube);

                    requestUri = $"https://www.googleapis.com/youtube/v3/{endpoint}?key={youtubeService.ClientId}";
                    break;
            }

            // Check that there are optional params then loop through all 
            // the params and add them onto the request URL
            if (optionalParams != null)
                requestUri = optionalParams
                    .Where(param => !string.IsNullOrEmpty(param.Key) && !string.IsNullOrEmpty(param.Value))
                    .Aggregate(requestUri, (current, param) => current + "&" + param.Key + "=" + param.Value);

            try
            {
                return await Task.Run(async () =>
                {
                    // Create the client
                    using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }))
                    {
                        // We want json
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Add the user agent
                        client.DefaultRequestHeaders.UserAgent.Add(
                            new ProductInfoHeaderValue("SoundByte.Core", "1.0.0"));

                        // Add the service only if it's connected
                        if (IsServiceConnected(type))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", 
                                ServiceSecrets.FirstOrDefault(x => x.Service == type)?.UserToken.AccessToken);
                        }

                        // escape the url
                        var escapedUri = new Uri(Uri.EscapeUriString(requestUri));

                        // Get the URL
                        using (var webRequest = await client.GetAsync(escapedUri))
                        {
                            // This request has to be successful
                            webRequest.EnsureSuccessStatusCode();

                            // Get the body of the request as a stream
                            using (var stream = await webRequest.Content.ReadAsStreamAsync())
                            {
                                // Read the stream
                                using (var streamReader = new StreamReader(stream))
                                {
                                    // Get the text from the stream
                                    using (var textReader = new JsonTextReader(streamReader))
                                    {
                                        // Used to get the data from JSON
                                        var serializer =
                                            new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
                                        // Return the data
                                        return serializer.Deserialize<T>(textReader);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (OperationCanceledException)
            {
                return default(T);
            }
            catch (JsonSerializationException jsex)
            {
                throw new SoundByteException("JSON ERROR", jsex.Message, "\uEB63");
            }
            catch (Exception ex)
            {
                throw new SoundByteException("GENERAL ERROR", ex.Message, "\uE007");
            }
        }
        #endregion
    }
}
