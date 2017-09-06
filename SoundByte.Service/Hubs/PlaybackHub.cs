using Microsoft.AspNet.SignalR;
using SoundByte.Core.Items.Track;

namespace SoundByte.Service.Hubs
{
    public class PlaybackHub : Hub
    {
        public void PushTrack(BaseTrack track)
        {
             
        }
    }
}