using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NHub.Server.Startup))]

namespace NHub.Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}