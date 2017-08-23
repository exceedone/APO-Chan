using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Apo_ChanService.DataObjects;
using Apo_ChanService.Models;

namespace Apo_ChanService.Controllers
{
    [Authorize]
    public class ReportController : BaseController<ReportItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Apo_ChanContext context = new Apo_ChanContext();
            DomainManager = new EntityDomainManager<ReportItem>(context, Request);
        }

        // GET tables/report
        [HttpGet]
        public IQueryable<ReportItem> GetAllReportItems()
        {
           return Query();
        }

        // GET tables/report/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpGet]
        public SingleResult<ReportItem> GetReportItem(string id)
        {
            return Lookup(id);
        }

        // POST tables/report
        [HttpPost]
        public async Task<IHttpActionResult> PostReportItem(ReportItem item)
        {
            ReportItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // PATCH tables/report/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpPatch]
        public Task<ReportItem> PatchReportItem(string id, Delta<ReportItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // DELETE tables/report/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpDelete]
        public Task DeleteReportItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}