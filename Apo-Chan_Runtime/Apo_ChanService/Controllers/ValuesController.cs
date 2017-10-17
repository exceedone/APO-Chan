using System.Web.Http;
using System.Web.Http.Tracing;
using System.Web.Http.Controllers;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Apo_ChanService.DataObjects;
using Apo_ChanService.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Apo_ChanService.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    //[CustomAttributes.CustomAuthentize]
    public class ValuesController : ApiController
    {
        Apo_ChanContext context;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            this.context = new Apo_ChanContext();
        }

        // GET api/values
        //public string Get()
        //{
        //    MobileAppSettingsDictionary settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
        //    ITraceWriter traceWriter = this.Configuration.Services.GetTraceWriter();

        //    string host = settings.HostName ?? "localhost";
        //    string greeting = "Hello from " + host;

        //    traceWriter.Info(greeting);
        //    return greeting;
        //}

        //// POST api/values
        //public string Post()
        //{
        //    return "Hello World!";
        //}

        //[HttpGet]
        //[Route("api/values/groupandusers/{id}")]
        //public async Task<IHttpActionResult> GetGroupAndUsers(string id)
        //{
        //    var joins = this.context.GroupUserItems
        //        .Where(x => x.RefUserId == id && !x.Deleted)
        //        .Join(context.GroupItems, user => user.RefGroupId, group => group.Id
        //        , (user, group) => new { user, group }
        //        ).ToList();
        //    return null;
        //}

        //[HttpGet]
        //[Route("api/values/signout")]
        //public IHttpActionResult SignOut()
        //{
        //    return Redirect("apochan-scheme://signout");
        //}

    }
}
