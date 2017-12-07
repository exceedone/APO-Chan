using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;

namespace Apo_Chan.Managers
{
    /// <summary>
    /// "UserManager" is same name in  namespace "Android.OS".
    /// So rename User"s"Manager.
    /// </summary>
    public partial class ReportGroupManager : BaseManager<ReportGroupItem>
    {
        static ReportGroupManager()
        {
            defaultInstance = new ReportGroupManager();
        }
        public ReportGroupManager() : base() { }

        public static ReportGroupManager DefaultManager
        {
            get
            {
                return defaultInstance as ReportGroupManager;
            }
            private set
            {
                defaultInstance = value;
            }
        }
        
        public async Task<ReportGroupItem> GetItemAsync(Expression<Func<ReportGroupItem, bool>> expression)
        {
            // get from Azure Mobile Apps
            try
            {
                // not token update info
                //await BaseAuthProvider.RefreshProfile();
                IEnumerable<ReportGroupItem> items = await this.localDataTable
                    .Where(expression)
                    .ToEnumerableAsync();

                if (!items.Any()) { return null; }
                return items.ToList().FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                DebugUtil.WriteLine("ReportGroupManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine("ReportGroupManager Sync error: " + e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<ReportGroupItem>> GetItemsAsync(Expression<Func<ReportGroupItem, bool>> expression)
        {
            // get from Azure Mobile Apps
            try
            {
                // not token update info
                //await BaseAuthProvider.RefreshProfile();
                IEnumerable<ReportGroupItem> items = await this.localDataTable
                    .Where(expression)
                    .ToEnumerableAsync();

                ObservableCollection<ReportGroupItem> reportgroups = new ObservableCollection<ReportGroupItem>();
                foreach (var item in items)
                {
                    reportgroups.Add(item);
                }

                return reportgroups;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                DebugUtil.WriteLine("ReportGroupManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine("ReportGroupManager Sync error: " + e.Message);
            }
            return null;
        }

        public override async Task SyncAsync()
        {
            try
            {
                ObservableCollection<string> groupList;
                groupList = await GroupUserManager.DefaultManager.GetGroupIdList();

                var query = localDataTable.Where(x => groupList.Contains(x.RefGroupId));
                await this.localDataTable.PullAsync(this.SyncQueryName, query);

                Service.OfflineSync.SyncResult.SyncedItems++;
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine($"{this.SyncQueryName} Manager PullAsync error: " + e.Message);
                Service.OfflineSync.SyncResult.OfflineSyncErrors.Add(Tuple.Create(SyncQueryName, 1));
            }
        }

        //sync support
        public async Task<ObservableCollection<string>> GetReportIdList()
        {
            ObservableCollection<string> reportList = new ObservableCollection<string>();

            IEnumerable<ReportGroupItem> items = await this.localDataTable
                    .Where(x => !x.Deleted)
                    .OrderBy(x => x.RefReportId)
                    .ToEnumerableAsync();
            ReportGroupItem prevReport = new ReportGroupItem
            {
                RefGroupId = string.Empty,
                RefReportId = string.Empty
            };
            foreach (var item in items)
            {
                if (item.RefReportId != prevReport.RefReportId)
                {
                    reportList.Add(item.RefReportId);
                    prevReport = item;
                }
            }

            return reportList;
        }

        public async Task DeleteGroupAsync(GroupItem group)
        {
            ObservableCollection<ReportGroupItem> existedGroup = await GetItemsAsync(x => x.RefGroupId == group.Id);

            foreach (var item in existedGroup)
            {
                await DeleteAsync(item);
            }
        }

        public async Task DeleteReportAsync(ReportItem report)
        {
            ObservableCollection<ReportGroupItem> existedReport = await GetItemsAsync(x => x.RefReportId == report.Id);

            foreach (var item in existedReport)
            {
                await DeleteAsync(item);
            }
        }

        public async Task DeleteAsync(string id)
        {
            await BaseAuthProvider.RefreshProfile();
            ReportGroupItem reportgroup = await base.LookupAsync(id);

            await localDataTable.DeleteAsync(reportgroup);
        }

        //Client implementation of "table/reportgroup/list/{reportid}"
        public async Task UpsertReport(string reportId, ObservableCollection<ReportGroupItem> reportGroupItems)
        {
            ObservableCollection<ReportGroupItem> existedReport = await this.GetItemsAsync(x => x.RefReportId == reportId);

            List<string> existedReportGroupId = new List<string>();

            foreach (var item in reportGroupItems)
            {
                var i = existedReport.FirstOrDefault(x => x.RefGroupId == item.RefGroupId);
                if (i == null)
                {
                    await SaveTaskAsync(item);
                }
                else
                {
                    existedReportGroupId.Add(i.Id);
                }
            }

            foreach (var rg in existedReport)
            {
                if (!existedReportGroupId.Contains(rg.Id))
                {
                    await DeleteAsync(rg.Id);
                }
            }
        }

        //Wrapper for UpsertReport(string reportId, ObservableCollection<ReportGroupItem> reportGroupItems)
        public async Task UpsertReport(string reportId, string groupIds)
        {
            string[] groupIdList = groupIds.Split(',');

            ObservableCollection<ReportGroupItem> reportGroupItems
                = await this.GetItemsAsync(x => x.RefReportId == reportId && groupIdList.Contains(x.RefGroupId));

            await this.UpsertReport(reportId, reportGroupItems);
        }

        //Client implementation of "api/values/groupsbyreport/{reportid}"
        public async Task<ObservableCollection<GroupItem>> GetGroupsByReport(string reportId)
        {
            ObservableCollection<ReportGroupItem> reportGroupItems = await this.GetItemsAsync(x => x.RefReportId == reportId);
            List<string> groupIds = new List<string>();
            foreach (var item in reportGroupItems)
            {
                groupIds.Add(item.RefGroupId);
            }

            ObservableCollection<GroupItem> groups = new ObservableCollection<GroupItem>();
            if (groupIds.Any())
            {
                groups = await GroupManager.DefaultManager.GetItemsAsync(x => groupIds.Contains(x.Id));
            }

            return groups;
        }

        //Client implementation of "api/values/reportsbygroup/{groupid}/{year}/{month}"
        public async Task<ObservableCollection<ReportItem>> GetReportsByGroup(string groupId, int year, int month)
        {
            ObservableCollection<ReportGroupItem> reportGroupItems = await this.GetItemsAsync(x => x.RefGroupId == groupId);
            List<string> reportIds = new List<string>();
            foreach (var item in reportGroupItems)
            {
                reportIds.Add(item.RefReportId);
            }

            ObservableCollection<ReportItem> reports = await ReportManager.DefaultManager.GetItemsAsync(x => reportIds.Contains(x.Id));
            return reports;
        }

    }
}
