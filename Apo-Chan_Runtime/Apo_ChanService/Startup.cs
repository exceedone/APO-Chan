using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

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