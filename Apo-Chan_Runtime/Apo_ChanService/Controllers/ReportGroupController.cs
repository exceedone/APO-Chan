using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Apo_ChanService.DataObjects;
using Apo_ChanService.Models;

namespace Apo_ChanService.Controllers
{
    [CustomAttributes.CustomAuthentize]
    public class ReportGroupController : BaseController<ReportGroupItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            base.context = new Apo_ChanContext();
            DomainManager = new EntityDomainManager<ReportGroupItem>(base.context, Request);
        }

        // GET tables/report
        [HttpGet]
        public IQueryable<ReportGroupItem> GetAllReportGroupItems()
        {
           return Query();
        }

        // GET tables/report/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpGet]
        public SingleResult<ReportGroupItem> GetReportGroupItem(string id)
        {
            return Lookup(id);
        }

        // POST tables/report
        [HttpPost]
        public async Task<IHttpActionResult> PostReportGroupItem(ReportGroupItem item)
        {
            ReportGroupItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // PATCH tables/report/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpPatch]
        public Task<ReportGroupItem> PatchReportGroupItem(string id, Delta<ReportGroupItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // DELETE tables/report/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpDelete]
        public Task DeleteReportGroupItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}