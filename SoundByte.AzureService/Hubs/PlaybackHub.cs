using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SoundByteBackendService.Hubs
{
    public class PlaybackHub : Hub
    {


        public void Hello()
        {
            Clients.All.hello();
        }
    }
}