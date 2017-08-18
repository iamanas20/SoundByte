using Microsoft.AspNet.SignalR;
using SoundByte.API.Endpoints;

namespace SoundByte.Service.Hubs
{
    public class PlaybackHub : Hub
    {
        public void PushTrack(Track track)
        {
             
        }
    }
}