using Microsoft.AspNet.SignalR;

namespace SoundByte.Service.Hubs
{
    public class PlaybackHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}