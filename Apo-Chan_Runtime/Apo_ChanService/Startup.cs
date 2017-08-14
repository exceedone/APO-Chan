using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Apo_ChanService.Startup))]

namespace Apo_ChanService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}