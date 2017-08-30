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
    public class UserController : BaseController<UserItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Apo_ChanContext context = new Apo_ChanContext();
            DomainManager = new EntityDomainManager<UserItem>(context, Request);
        }

        // GET tables/user
        [HttpGet]
        public IQueryable<UserItem> GetAllUserItems()
        {
            return Query();
        }

        // GET tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpGet]
        public SingleResult<UserItem> GetUserItem(string id)
        {
            return Lookup(id);
        }

        // POST tables/user
        [HttpPost]
        public async Task<IHttpActionResult> PostUserItem(UserItem item)
        {
            try { 
            UserItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }catch(System.Exception ex)
            {
                throw;
            }
        }

        // PATCH tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpPatch]
        public Task<UserItem> PatchUserItem(string id, Delta<UserItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // DELETE tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpDelete]
        public Task DeleteUserItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}