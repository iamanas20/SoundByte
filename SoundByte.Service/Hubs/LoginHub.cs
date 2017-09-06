using Microsoft.AspNet.SignalR;
using SoundByte.API.Items;

namespace SoundByte.Service.Hubs
{
    public class LoginHub : Hub
    {
        public void Connect(string code)
        {
            Groups.Add(Context.ConnectionId, code);
        }

        public void Disconnect(string code)
        {
            Groups.Remove(Context.ConnectionId, code);
        }
        public void SendLoginInfo(LoginToken info)
        {
            Clients.Group(info.LoginCode).RecieveLoginInfo<LoginToken>(info);
        }
    }
}