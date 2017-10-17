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
    [CustomAttributes.CustomAuthentize]
    public class GroupController : BaseController<GroupItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            base.context = new Apo_ChanContext();
            DomainManager = new EntityDomainManager<GroupItem>(base.context, Request);
        }

        // GET tables/user
        [HttpGet]
        public IQueryable<GroupItem> GetAllGroupItems()
        {
            return Query();
        }

        // GET tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpGet]
        public SingleResult<GroupItem> GetGroupItem(string id)
        {
            return Lookup(id);
        }

        // POST tables/user
        [HttpPost]
        public async Task<IHttpActionResult> PostGroupItem(GroupItem item)
        {
            try
            {
                GroupItem current = await InsertAsync(item);

                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (System.Web.Http.HttpResponseException ex)
            {
                string code = await ex.Response.Content.ReadAsStringAsync();
                throw;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        // PATCH tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpPatch]
        public Task<GroupItem> PatchGroupItem(string id, Delta<GroupItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // DELETE tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpDelete]
        public Task DeleteGroupItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}