using System.Linq;
using System.Collections.Generic;
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
    public class GroupUserController : BaseController<GroupUserItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            base.context = new Apo_ChanContext();
            DomainManager = new EntityDomainManager<GroupUserItem>(base.context, Request);
        }

        // GET tables/user
        [HttpGet]
        public IQueryable<GroupUserItem> GetAllGroupUserItems()
        {
            return Query();
        }

        // GET tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpGet]
        public SingleResult<GroupUserItem> GetGroupUserItem(string id)
        {
            return Lookup(id);
        }

        // GET tables/groupuser/group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpGet]
        [Route("tables/group/{groupid}/groupuser")]
        public IQueryable<GroupUserItem> GetGroupUserInGroupItem(string groupid)
        {
            return Query()
                .Where(x => x.RefGroupId == groupid && !x.Deleted)
            ;
        }

        // POST tables/user
        [HttpPost]
        public async Task<IHttpActionResult> PostGroupUserItem(GroupUserItem item)
        {
            try
            {
                GroupUserItem current = await InsertAsync(item);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        // PATCH tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpPatch]
        public Task<GroupUserItem> PatchGroupUserItem(string id, Delta<GroupUserItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // DELETE tables/user/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [HttpDelete]
        public Task DeleteGroupUserItem(string id)
        {
            return DeleteAsync(id);
        }

        /// <summary>
        /// Post Group User List, If not exists item, remove from db.
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("table/groupuser/list/{groupid}")]
        public async Task<IHttpActionResult> PostReportGroupList(string groupid, [FromBody]List<GroupUserItem> list)
        {
            // get items from reportid.
            List<GroupUserItem> dbItems = this.context.GroupUserItems
                .Where(x => x.RefGroupId == groupid).ToList();
            List<string> existsIdList = new List<string>();
            // loop items
            foreach (var item in list)
            {
                // check parameter refUserId has db
                var i = dbItems.FirstOrDefault(x => x.RefUserId == item.RefUserId);
                // not exists : insert(as newItem)
                if (i == null)
                {
                    await InsertAsync(item);
                }
                // exists:add i.id (Exists item)
                else
                {
                    existsIdList.Add(i.Id);
                }
            }

            foreach (var d in dbItems)
            {
                // if d.Id not Exists existsIdList, remove from db.
                if (!existsIdList.Contains(d.Id))
                {
                    await DeleteAsync(d.Id);
                }
            }

            return Ok();
        }

    }
}