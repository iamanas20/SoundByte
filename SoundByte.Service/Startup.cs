using Microsoft.Owin;
using Owin;

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