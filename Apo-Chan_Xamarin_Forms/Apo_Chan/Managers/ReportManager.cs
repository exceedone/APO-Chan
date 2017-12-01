using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Apo_Chan.Managers
{
    public partial class ReportManager : BaseManager<ReportItem>
    {
        //public override string SyncQueryName
        //{
        //    get { return "allReportItems"; }
        //}

        static ReportManager()
        {
            defaultInstance = new ReportManager();
        }

        public ReportManager() : base() { }

        public static ReportManager DefaultManager
        {
            get
            {
                return defaultInstance as ReportManager;
            }
            private set
            {
                defaultInstance = value;
            }
        }
        public async Task<ObservableCollection<ReportItem>> GetItemsAsync(int year, int month)
        {
            // get from Azure Mobile Apps
            try
            {
                await BaseAuthProvider.RefreshProfile();

                var user = GlobalAttributes.User;
                var userid = user.Id;
                IEnumerable<ReportItem> items = await this.localDataTable
                    .Where(x =>
                        x.RefUserId == userid
                        //&& !x.Deleted
                        && ((x.ReportStartDate.Year == year && x.ReportStartDate.Month == month)
                           || (x.ReportEndDate.Year == year && x.ReportEndDate.Month == month))
                    ).OrderBy(x => x.ReportStartDate).ThenBy(x => x.ReportEndDate)
                    .ThenBy(x => x.ReportStartTime).ThenBy(x => x.ReportEndTime)
                    .ToEnumerableAsync();

                ObservableCollection<ReportItem> reports = new ObservableCollection<ReportItem>();
                foreach (var item in items)
                {
                    //for local database, Deleted is null in dataTable when new report submitted
                    //if (!item.Deleted)
                    {
                        Utils.ConvertToLocalDateTime(item);
                        reports.Add(item);
                    }
                }

                return reports;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                DebugUtil.WriteLine("ReportManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine("ReportManager Sync error: " + e.Message);
            }
            return null;
        }

        public new async Task SaveTaskAsync(ReportItem report)
        {
            await BaseAuthProvider.RefreshProfile();
            Utils.ConvertToUtcDateTime(report);
            await base.SaveTaskAsync(report);
        }

        public new async Task<ReportItem> LookupAsync(string id)
        {
            await BaseAuthProvider.RefreshProfile();
            ReportItem report = await base.LookupAsync(id);
            Utils.ConvertToLocalDateTime(report);

            return report;
        }

        public override async Task SyncAsync()
        {
            IMobileServiceTableQuery<ReportItem> query;
            try
            {
                query = localDataTable.Where(x => x.RefUserId == GlobalAttributes.User.UserProviderId);

                await this.localDataTable.PullAsync(this.SyncQueryName, query);
                Service.OfflineSync.SyncResult.SyncedItems++;
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine($"{this.SyncQueryName} Manager PullAsync error: " + e.Message);
                Service.OfflineSync.SyncResult.OfflineSyncErrors.Add(Tuple.Create(SyncQueryName, 1));
            }
        }

    }
}
