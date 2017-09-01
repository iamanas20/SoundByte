using Microsoft.AspNet.SignalR;
using SoundByte.API.Items.Track;

namespace SoundByte.Service.Hubs
{
    public class PlaybackHub : Hub
    {
        public void PushTrack(BaseTrack track)
        {
             
        }
    }
}