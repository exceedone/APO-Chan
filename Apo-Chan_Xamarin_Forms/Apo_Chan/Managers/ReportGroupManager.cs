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

        public override async Task SyncAsync()
        {
            try
            {
                foreach (var item in GroupUserManager.DefaultManager.GroupList)
                {
                    //pull ReportGroup by each group id
                    var query = localDataTable.Where(x => x.RefGroupId == item.RefGroupId);
                    await this.localDataTable.PullAsync(this.SyncQueryName + item.RefGroupId, query);
                }

                Service.OfflineSync.SyncResult.SyncedItems++;
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine($"{this.SyncQueryName} Manager PullAsync error: " + e.Message);
                Service.OfflineSync.SyncResult.OfflineSyncErrors.Add(Tuple.Create(SyncQueryName, 1));
            }
        }

        public async Task<ObservableCollection<ReportGroupItem>> GetReportList()
        {
            ObservableCollection<ReportGroupItem> reportList = new ObservableCollection<ReportGroupItem>();

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
                    reportList.Add(item);
                    prevReport = item;
                }
            }

            return reportList;
        }

    }
}
