using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using SoundByte.API.Endpoints;

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
        public void SendLoginInfo(LoginInfo info)
        {
            Clients.Group(info.LoginCode).RecieveLoginInfo<LoginInfo>(info);
        }
    }
}