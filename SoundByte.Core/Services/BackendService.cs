using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using SoundByte.Core.API.Endpoints;
using SoundByte.Core.Helpers;

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

        public BackendService()
        {
            _mobileService = new MobileServiceClient(_backendAzureServiceUrl);
            _mobileHub = new HubConnection(_backendAzureServiceUrl);

            if (_mobileService.CurrentUser != null)
            {
                _mobileHub.Headers["x-zumo-auth"] = _mobileService.CurrentUser.MobileServiceAuthenticationToken;
            }

            AsyncHelper.RunSync(async () =>
            {
                try
                {
                    await _mobileHub.Start();
                }
                catch
                {
                    // ignored
                }
            });   
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

            if (_mobileHub.State == ConnectionState.Connected)
            {
                var trackString = JsonConvert.SerializeObject(track);
                await _mobileHub.Send(trackString);
            }
        }
    }
}
