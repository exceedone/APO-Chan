using System.Web.Http;
using System.Web.Http.Tracing;
using System.Web.Http.Controllers;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Apo_ChanService.DataObjects;
using Apo_ChanService.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

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

        /// <summary>
        /// Select user id, and get groups the user joins.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/values/userjoingroups/{userid}")]
        public IHttpActionResult GetUserJoinGroups(string userid)
        {
            List<dynamic> list = new List<dynamic>();
            // get groups the user joins
            var items = this.context.GroupUserItems
                .Where(x => x.RefUserId == userid && !x.Deleted)
                .Join(context.GroupItems, user => user.RefGroupId, group => group.Id
                , (user, group) => new { user, group }
                ).ToList();

            // get user count
            foreach (var item in items)
            {
                var usersCount = this.context.GroupUserItems
                    .Where(x => x.RefGroupId == item.group.Id && !x.Deleted)
                    .Count();

                list.Add(new
                {
                    group = item.group
                    ,
                    adminflg = item.user.AdminFlg
                    ,
                    usercount = usersCount
                });
            }

            return Json(list);
        }

        /// <summary>
        /// Select group id, and get users list.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("api/values/groupusers/{groupid}")]
        //public IHttpActionResult GetGroupUsers(string groupid)
        //{
        //    List<dynamic> list = new List<dynamic>();
        //    // get groups the groupuser and user joins
        //    var items = this.context.GroupUserItems
        //        .Where(x => x.RefGroupId == groupid && !x.Deleted)
        //        .Join(context.UserItems, groupuser => groupuser.RefUserId, user => user.Id
        //        , (groupuser, user) => new { groupuser, user }
        //        ).Join(context.GroupItems, usergroupuser => usergroupuser.groupuser.RefGroupId, group => group.Id
        //        , (usergroupuser, group) => new { usergroupuser, group }
        //        ).ToList();

        //    // select usergrouplist
        //    var g = items.FirstOrDefault().group;
        //    var groupUserList = items.Select(x => x.usergroupuser).ToList();

        //    return Json(new { group = g, users = groupUserList });
        //}

        [HttpGet]
        [Route("api/values/groupusers/{groupid}")]
        public IHttpActionResult GetGroupUsers(string groupid)
        {
            List<dynamic> list = new List<dynamic>();
            // get groups the groupuser and user joins
            var items = this.context.GroupUserItems
                .Where(x => x.RefGroupId == groupid && !x.Deleted)
                .Join(context.UserItems, groupuser => groupuser.RefUserId, user => user.Id
                , (groupuser, user) => new { groupuser, user }
                ).Join(context.GroupItems, usergroupuser => usergroupuser.groupuser.RefGroupId, group => group.Id
                , (usergroupuser, group) => new { usergroupuser, group }
                ).ToList();

            // select usergrouplist
            var g = items.FirstOrDefault().group;
            var groupusers = items.Select(x => x.usergroupuser.groupuser).ToList();

            return Json(new { group = g, groupusers = groupusers });
        }
    }
}
