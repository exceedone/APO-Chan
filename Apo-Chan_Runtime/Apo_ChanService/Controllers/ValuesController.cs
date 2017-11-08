using System;
using System.Web.Http;
using System.Web.Http.Tracing;
using System.Web.Http.Controllers;
using Microsoft.Azure;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Apo_ChanService.DataObjects;
using Apo_ChanService.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Apo_ChanService.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    [RoutePrefix("api/values")]
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
        /// Get Blob signature.
        /// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-dotnet-shared-access-signature-part-2#part-1-create-a-console-application-to-generate-shared-access-signatures
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobname"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("blobsignature/{containername}")]
        public IHttpActionResult GetBlobSignature(string containername)
        {
            if (string.IsNullOrWhiteSpace(containername))
            {
                return BadRequest();
            }

            string policyName = "apochanpolicy" + containername;
            DateTimeOffset date = DateTimeOffset.UtcNow.AddHours(24);

            //Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.ConnectionStrings["MS_AzureStorageAccountConnectionString"].ToString());
            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference(containername);

            //Clear any existing access policies on container.
            BlobContainerPermissions perms = container.GetPermissions();
            perms.SharedAccessPolicies.Clear();
            container.SetPermissions(perms);

            //Get the container's existing permissions.
            BlobContainerPermissions permissions = container.GetPermissions();
            //Create a new shared access policy and define its constraints.
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = date,
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read
            };

            //Add the new policy to the container's permissions, and set the container's permissions.
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            container.SetPermissions(permissions);

            //Generate the shared access signature on the container. In this case, all of the constraints for the
            //shared access signature are specified on the stored access policy.
            string sasContainerToken = container.GetSharedAccessSignature(null, policyName);

            //Return the URI string for the container, including the SAS token.
            string url = container.Uri + sasContainerToken;

            //Return the URI string for the container, including the SAS token.
            return Content(System.Net.HttpStatusCode.OK, new { ContainerName = containername , Signature = url, Expire = date });
        }

        /// <summary>
        /// Select user id, and get groups the user joins.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("userjoingroups/{userid}")]
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
        [Route("groupusers/{groupid}")]
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
            var g = items.FirstOrDefault()?.group;
            var groupusers = items.Select(x => x.usergroupuser.groupuser).ToList();

            return Json(new { group = g, groupusers = groupusers });
        }

        /// <summary>
        /// Get Reports, key groupid.
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("reportsbygroup/{groupid}/{year}/{month}")]
        public IHttpActionResult GetReportsByGroup(string groupid, int year, int month)
        {
            List<dynamic> list = new List<dynamic>();
            // get reports the reportgroup and group joins
            var items = this.context.GroupItems
                .Where(x => x.Id == groupid && !x.Deleted)
                .Join(context.ReportGroupItems, group => group.Id, reportgroup => reportgroup.RefGroupId
                , (group, reportgroup) => new { group, reportgroup }
                ).Join(context.ReportItems, reportgroup => reportgroup.reportgroup.RefReportId, report => report.Id
                , (reportgroup, report) => report
                ).Join(context.UserItems, report => report.RefUserId, user => user.Id
                , (report, user) => new { report, user }
                ).Where(x => (x.report.ReportStartDate.Value.Year == year && x.report.ReportStartDate.Value.Month == month) || (x.report.ReportEndDate.Value.Year == year && x.report.ReportEndDate.Value.Month == month) && !x.report.Deleted).ToList();

            //return Json(new { group = g, groupusers = groupusers });
            return Json(items.Select(x => x.report));
        }

        /// <summary>
        /// Get Reports, key groupid.
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("groupsbyreport/{reportid}")]
        public IHttpActionResult GetGroupsByReport(string reportid)
        {
            List<dynamic> list = new List<dynamic>();
            // get reports the reportgroup and group joins
            var items = this.context.ReportItems
                .Where(x => x.Id == reportid && !x.Deleted)
                .Join(context.ReportGroupItems, report => report.Id, reportgroup => reportgroup.RefReportId
                , (report, reportgroup) => new { report, reportgroup }
                ).Join(context.GroupItems, reportgroup => reportgroup.reportgroup.RefGroupId, group => group.Id
                , (reportgroup, group) => group
                );

            //return Json(new { group = g, groupusers = groupusers });
            return Json(items);
        }

    }
}
