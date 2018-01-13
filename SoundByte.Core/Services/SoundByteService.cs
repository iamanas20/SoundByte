/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
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
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.User;
using SoundByte.Core.Items.YouTube;

namespace SoundByte.Core.Services
{
    /// <summary>
    /// Next generation (Gen3.0) SoundByte Service. New features include portability (.NET Standard),
    /// events (event based e.g OnServiceConnected), muiltiple services, easy to extend.
    /// </summary>
    public class SoundByteService
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
        /// List of services and their client id / client secrets.
        /// Also contains login information.
        /// </summary>
        public List<ServiceInfo> Services { get; } = new List<ServiceInfo>();

        #endregion

        #region Getters and Setters

        #endregion

        #region Instance Setup
        private static readonly Lazy<SoundByteService> InstanceHolder =
            new Lazy<SoundByteService>(() => new SoundByteService());

        /// <summary>
        /// Gets the current instance of SoundByte V3 Service
        /// </summary>
        public static SoundByteService Current => InstanceHolder.Value;

        /// <summary>
        /// Setup the service
        /// </summary>
        /// <param name="services">A list of services that will be used in the app</param>
        public void Init(IEnumerable<ServiceInfo> services)
        {
            // Empty any other secrets
            Services.Clear();

            // Loop through all the keys and add them
            foreach (var service in services)
            {
                // If there is already a service in the list, thow an exception, there
                // should only be one key for each service.
                if (Services.FirstOrDefault(x => x.Service == service.Service) != null)
                    throw new Exception("Only one key for each service!");

                Services.Add(service);
            }

            _isLoaded = true;
        }

        /// <summary>
        /// Init logged in user objects. This should be called in the background after the app
        /// has started.
        /// </summary>
        /// <returns></returns>
        public async Task InitUsersAsync()
        {
            foreach (var service in Services)
            {
                // Don't run if the user has not logged in
                if (service.UserToken == null) continue;

                try
                {
                    switch (service.Service)
                    {
                        case ServiceType.Fanburst:
                            service.CurrentUser = (await GetAsync<FanburstUser>(ServiceType.Fanburst, "/me").ConfigureAwait(false)).ToBaseUser();
                            break;
                        case ServiceType.SoundCloud:
                        case ServiceType.SoundCloudV2:
                            service.CurrentUser = (await GetAsync<SoundCloudUser>(ServiceType.SoundCloud, "/me").ConfigureAwait(false)).ToBaseUser();
                            break;
                        case ServiceType.YouTube:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<YouTubeChannelHolder>(ServiceType.YouTube, "/channels", new Dictionary<string, string>
                            {
                                { "mine", "true" },
                                { "part", "snippet" }
                            }).ConfigureAwait(false)).Channels.FirstOrDefault()?.ToBaseUser();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not load user: " + ex.Message);
                }
            }
        }

        #endregion

        #region Service Methods
        /// <summary>
        /// Returns the user object of the connected servive. Please note,
        /// this value can be null if the user has not logged in.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [CanBeNull]
        public BaseUser GetConnectedUser(ServiceType type)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Check that the service actually exists
            var service = Services.FirstOrDefault(x => x.Service == type);
            if (service == null)
                throw new ServiceDoesNotExistException(type);

            // If the user token is not null, but the user is null, update the user
            if (service.UserToken != null && service.CurrentUser == null)
            {
                try
                {
                    switch (service.Service)
                    {
                        case ServiceType.Fanburst:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<FanburstUser>(ServiceType.Fanburst, "/me").ConfigureAwait(false)).ToBaseUser();
                            break;
                        case ServiceType.SoundCloud:
                        case ServiceType.SoundCloudV2:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<SoundCloudUser>(ServiceType.SoundCloud, "/me").ConfigureAwait(false)).ToBaseUser();
                            break;
                        case ServiceType.YouTube:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<YouTubeChannelHolder>(ServiceType.YouTube, "/channels", new Dictionary<string, string>
                            {
                                { "mine", "true" },
                                { "part", "snippet" }
                            }).ConfigureAwait(false)).Channels.FirstOrDefault()?.ToBaseUser();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not load user: " + ex.Message);
                }
            }

            // Return the connected user
            return service.CurrentUser;
        }

        /// <summary>
        /// Connects a service to SoundByte. This will allow accessing
        /// user content. The ServiceConnected event is fired.
        /// </summary>
        /// <param name="type">The service to connect.</param>
        /// <param name="token">The required token</param>
        public void ConnectService(ServiceType type, LoginToken token)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            var service = Services.FirstOrDefault(x => x.Service == type);
            if (service == null)
                throw new ServiceDoesNotExistException(type);

            // Set the token
            service.UserToken = token;

            if (service.UserToken != null)
            {
                try
                {
                    switch (service.Service)
                    {
                        case ServiceType.Fanburst:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<FanburstUser>(ServiceType.Fanburst, "/me").ConfigureAwait(false)).ToBaseUser();
                            break;
                        case ServiceType.SoundCloud:
                        case ServiceType.SoundCloudV2:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<SoundCloudUser>(ServiceType.SoundCloud, "/me").ConfigureAwait(false)).ToBaseUser();
                            break;
                        case ServiceType.YouTube:
                            // Do this later
                            break;
                    }
                }
                catch
                {
                    // Todo: There are many reasons why this could fail.
                    // For now we just delete the user token
                    service.UserToken = null;
                }
            }

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
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Get the service information
            var service = Services.FirstOrDefault(x => x.Service == type);
            if (service == null)
                throw new ServiceDoesNotExistException(type);

            // Delete the user token
            service.UserToken = null;
            service.CurrentUser = null;

            // Fire the event
            OnServiceDisconnected?.Invoke(type);
        }

        /// <summary>
        ///     Has the user logged in with their SoundByte account.
        /// </summary>
        public bool IsSoundByteAccountConnected => IsServiceConnected(ServiceType.SoundByte);

        /// <summary>
        /// Is the user logged into a service. Warning: will throw an exception if
        /// the service does not exsit.
        /// </summary>
        /// <param name="type">The service to check if the user has connected.</param>
        /// <returns>If the user accounted is connected</returns>
        public bool IsServiceConnected(ServiceType type)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Get the service information
            var service = Services.FirstOrDefault(x => x.Service == type);

            if (service == null)
                throw new ServiceDoesNotExistException(type);

            // If the user token is not null, we are connected
            return service.UserToken != null;
        }
        #endregion

        #region Web API
        /// <summary>
        ///     This method builds the request url for the specified service.
        /// </summary>
        /// <param name="type">The service type to build the request url</param>
        /// <param name="endpoint">User defiend endpoint</param>
        /// <param name="optionalParams"></param>
        /// <returns>Fully build request url</returns>
        private string BuildRequestUrl(ServiceType type, string endpoint, [CanBeNull] Dictionary<string, string> optionalParams)
        {
            // Start building the request URL
            string requestUri;

            // Strip out the / infront of the endpoint if it exists
            endpoint = endpoint.TrimStart('/');

            switch (type)
            {
                case ServiceType.SoundCloud:
                    var soundCloudService = Services.FirstOrDefault(x => x.Service == ServiceType.SoundCloud);
                    if (soundCloudService == null)
                        throw new ServiceDoesNotExistException(ServiceType.SoundCloud);

                    requestUri = $"https://api.soundcloud.com/{endpoint}?client_id={soundCloudService.ClientId}";
                    break;

                case ServiceType.SoundCloudV2:
                    var soundCloudV2Service = Services.FirstOrDefault(x => x.Service == ServiceType.SoundCloudV2);
                    if (soundCloudV2Service == null)
                        throw new ServiceDoesNotExistException(ServiceType.SoundCloudV2);

                    requestUri = $"https://api-v2.soundcloud.com/{endpoint}?client_id={soundCloudV2Service.ClientId}";
                    break;

                case ServiceType.Fanburst:
                    var fanburstService = Services.FirstOrDefault(x => x.Service == ServiceType.Fanburst);
                    if (fanburstService == null)
                        throw new ServiceDoesNotExistException(ServiceType.Fanburst);

                    requestUri = $"https://api.fanburst.com/{endpoint}?client_id={fanburstService.ClientId}";
                    break;
                case ServiceType.YouTube:
                    var youtubeService = Services.FirstOrDefault(x => x.Service == ServiceType.YouTube);
                    if (youtubeService == null)
                        throw new ServiceDoesNotExistException(ServiceType.YouTube);

                    requestUri = $"https://www.googleapis.com/youtube/v3/{endpoint}?key={youtubeService.ClientId}";
                    break;
                case ServiceType.ITunesPodcast:
                    requestUri = $"https://itunes.apple.com/{endpoint}?key=0";
                    break;
                case ServiceType.SoundByte:
                    var soundByteService = Services.FirstOrDefault(x => x.Service == ServiceType.SoundByte);
                    if (soundByteService == null)
                        throw new ServiceDoesNotExistException(ServiceType.SoundByte);

                    requestUri = $"https://soundbytemedia.com/api/v1/{endpoint}?client_id={soundByteService.ClientId}";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            // Check that there are optional params then loop through all 
            // the params and add them onto the request URL
            if (optionalParams != null)
                requestUri = optionalParams
                    .Where(param => !string.IsNullOrEmpty(param.Key) && !string.IsNullOrEmpty(param.Value))
                    .Aggregate(requestUri, (current, param) => current + "&" + param.Key + "=" + param.Value);

            return requestUri;
        }

        /// <summary>
        ///     Adds the required headers to the http service depending on
        ///     the service type. This defaults to OAuth, and uses Bearer for 
        ///     YouTube and Fanburst
        /// </summary>
        /// <param name="service">Http Service to append the headers.</param>
        /// <param name="type">What type of service is this user accessing.</param>
        private void BuildAuthLayer(HttpService service, ServiceType type)
        {
            // Add the service only if it's connected
            if (IsServiceConnected(type))
            {
                // Get the token
                var token = Services.FirstOrDefault(x => x.Service == type)?.UserToken?.AccessToken;

                // Add the auth request
                switch (type)
                {
                    case ServiceType.YouTube:
                    case ServiceType.Fanburst:
                    case ServiceType.SoundByte:
                        service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        break;
                    default:
                        service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                        break;
                }
            }
        }

        /// <summary>
        ///     Handles token refreshing when the access token expires
        /// </summary>
        /// <param name="hex">HttpRequestException exception</param>
        /// <param name="type">The service type</param>
        /// <returns>If we refreshed the refresh token</returns>
        private async Task<bool> HandleAuthTokenRefreshAsync(HttpRequestException hex, ServiceType type)
        {
            if (!hex.Message.ToLower().Contains("401") || !IsServiceConnected(type))
                return false;

            try
            {
                // Get the token
                var userToken = Services.FirstOrDefault(x => x.Service == type)?.UserToken;
                if (userToken != null)
                {
                    var newToken = await AuthorizationHelpers.GetNewAuthTokenAsync(type, userToken.RefreshToken);

                    userToken.AccessToken = newToken.AccessToken;
                    userToken.ExpireTime = newToken.ExpireTime;

                    // Reconnect the service
                    ConnectService(type, userToken);
                }
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Error obtaining new access token.", ex.Message);
            }

            // Return after a slight 
            await Task.Delay(500);
            return true;
        }

        /// <summary>
        ///     Fetches an object from the specified service API and returns it.
        /// </summary>
        public async Task<T> GetAsync<T>(ServiceType type, string endpoint, Dictionary<string, string> optionalParams = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, optionalParams);

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.GetAsync<T>(requestUri, cancellationTokenSource).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex) 
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await GetAsync<T>(type, endpoint, optionalParams, cancellationTokenSource);

                throw new SoundByteException("No connection?", hex.Message + "\n" + requestUri);
            }
        }

        /// <summary>
        ///     This method allows the ability to perform a PUT command at a certain API method. Also
        ///     adds required OAuth token.
        ///     Returns if the PUT request has successful or not
        /// </summary>
        public async Task<bool> PutAsync(ServiceType type, string endpoint, string content = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Start building the request URL
            var requestUri = BuildRequestUrl(type, endpoint, null);

            // We have to have content (ugh)
            if (string.IsNullOrEmpty(content))
                content = "n/a";

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.PutAsync(requestUri, content, cancellationTokenSource).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await PutAsync(type, endpoint, content, cancellationTokenSource);

                throw new SoundByteException("No connection?", hex.Message + "\n" + requestUri);
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Something went wrong", ex.Message);
            }
        }

        /// <summary>
        ///     Contacts the specified API and posts the content.
        /// </summary>
        public async Task<T> PostAsync<T>(ServiceType type, string endpoint, string content = null,
            Dictionary<string, string> optionalParams = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, optionalParams);

            // We have to have content (ugh)
            if (string.IsNullOrEmpty(content))
                content = "n/a";

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.PostAsync<T>(requestUri, content, cancellationTokenSource).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await PostAsync<T>(type, endpoint, content, optionalParams, cancellationTokenSource);

                throw new SoundByteException("No connection?", hex.Message + "\n" + requestUri);
            }
        }

        public async Task<T> PostItemAsync<T>(ServiceType type, string endpoint, T item, Dictionary<string, string> optionalParams = null,
            CancellationTokenSource cancellationTokenSource = null)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, optionalParams);

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.PostAsync<T>(requestUri, JsonConvert.SerializeObject(item), cancellationTokenSource).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await PostItemAsync(type, endpoint, item, optionalParams, cancellationTokenSource);

                throw new SoundByteException("No connection?", hex.Message + "\n" + requestUri);
            }
        }

        /// <summary>
        ///    Attempts to delete an object from the specified API
        /// </summary>
        public async Task<bool> DeleteAsync(ServiceType type, string endpoint, CancellationTokenSource cancellationTokenSource = null)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, null);

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.DeleteAsync(requestUri, cancellationTokenSource).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                try
                {
                    if (await HandleAuthTokenRefreshAsync(hex, type))
                        return await DeleteAsync(type, endpoint, cancellationTokenSource);
                }
                catch
                {
                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Checks to see if an items exists at the specified endpoint
        /// </summary>
        public async Task<bool> ExistsAsync(ServiceType type, string endpoint, CancellationTokenSource cancellationTokenSource = null)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, null);

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.ExistsAsync(requestUri, cancellationTokenSource).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                try
                {
                    if (await HandleAuthTokenRefreshAsync(hex, type))
                        return await ExistsAsync(type, endpoint, cancellationTokenSource);
                }
                catch
                {
                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
    #endregion
}