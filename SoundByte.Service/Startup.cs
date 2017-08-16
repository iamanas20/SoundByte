using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SoundByteBackendService.Startup))]

namespace SoundByteBackendService
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