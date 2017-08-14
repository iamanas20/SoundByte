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
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.WindowsAzure.MobileServices;
using SoundByte.Core.API.Endpoints;

namespace SoundByte.Core.Services
{
    public class BackendService
    {
        private static readonly Lazy<BackendService> InstanceHolder =
            new Lazy<BackendService>(() => new BackendService());

        public static BackendService Instance => InstanceHolder.Value;

        private string _backendAzureServiceUrl = "https://soundbytebackend.azurewebsites.net";

        private readonly MobileServiceClient _mobileService;
        private readonly HubConnection _mobileHub;
        private IHubProxy _playbackHub;

        private BackendService()
        {
            _mobileService = new MobileServiceClient(_backendAzureServiceUrl);
            _mobileHub = new HubConnection(_backendAzureServiceUrl);

            // Add user auth.
            if (_mobileService.CurrentUser != null)
            {
                _mobileHub.Headers["x-zumo-auth"] = _mobileService.CurrentUser.MobileServiceAuthenticationToken;
            }

            // Create the playback hub
            _playbackHub = _mobileHub.CreateHubProxy("PlaybackHub");
        }

        /// <summary>
        /// Pushes the current track up to the backend. This will allow the
        /// user to continue their current song in the future (after app restart, or onto
        /// another device). This is done as a track changes.
        /// </summary>
        /// <param name="track">The track to push up.</param>
        public async Task PushCurrentTrackAsync(Track track)
        {
            // Don't do this is the user is not logged in.
            if (!SoundByteService.Instance.IsAccountConnected)
                return;

            // Try connect if disconnected
            if (_mobileHub.State != ConnectionState.Connected)
                await _mobileHub.Start();

            // Only perform is connected
            if (_mobileHub.State == ConnectionState.Connected)
            {
              //todo
            }
        }
    }
}
