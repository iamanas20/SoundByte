using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using SoundByte.Core.Items;

namespace SoundByte.UWP.Services
{
    public class BackendService
    {
        private static readonly Lazy<BackendService> InstanceHolder =
            new Lazy<BackendService>(() => new BackendService());

        public static BackendService Instance => InstanceHolder.Value;

        private string _backendServiceUrl = "https://soundbytemedia.com";

        private readonly HubConnection _mobileHub;
        public IHubProxy LoginHub { get; }

        private BackendService()
        {
            _mobileHub = new HubConnection(_backendServiceUrl);

            LoginHub = _mobileHub.CreateHubProxy("LoginHub");
        }


        public async Task LoginXboxConnect(string code)
        {
            try
            {
                await Task.Run(async () =>
                {
                    // Try connect if disconnected
                    if (_mobileHub.State != ConnectionState.Connected)
                        await _mobileHub.Start();

                    // Only perform is connected
                    if (_mobileHub.State == ConnectionState.Connected)
                    {
                        await LoginHub.Invoke("Connect", code);
                    }
                    else
                    {
                        await NavigationService.Current.CallMessageDialogAsync("Could not Connect...");
                    }
                });
            }
            catch
            {
                await NavigationService.Current.CallMessageDialogAsync("Could not Connect...");
                // Do Nothing
            }
        }

        public async Task LoginXboxDisconnect(string code)
        {
            try
            {
                // Try connect if disconnected
                if (_mobileHub.State != ConnectionState.Connected)
                    await _mobileHub.Start();

                // Only perform is connected
                if (_mobileHub.State == ConnectionState.Connected)
                {
                    await LoginHub.Invoke("Disconnect", code);
                }
            }
            catch
            {
                // Do Nothing
            }
        }

        public async Task<string> LoginSendInfoAsync(LoginToken info)
        {
            try
            {
                // Try connect if disconnected
                if (_mobileHub.State != ConnectionState.Connected)
                    await _mobileHub.Start();

                // Only perform is connected
                if (_mobileHub.State == ConnectionState.Connected)
                {
                    await LoginHub.Invoke("SendLoginInfo", info);
                    return string.Empty;
                }

                return "Not Connected";     
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
