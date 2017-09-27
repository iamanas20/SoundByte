using System.Collections.Generic;
using Microsoft.Owin;
using Owin;
using SoundByte.Core.Items;
using SoundByte.Core.Services;

[assembly: OwinStartup(typeof(SoundByte.Service.Startup))]

namespace SoundByte.Service
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
            app.MapSignalR();
        }
    }
}