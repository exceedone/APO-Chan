using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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
        public async Task<ObservableCollection<ReportItem>> GetItemsAsync(int year, int month, bool syncItems = false)
        {
            // get from Azure Mobile Apps
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                await BaseAuthProvider.RefreshProfile();

                var user = GlobalAttributes.User;
                IEnumerable<ReportItem> items = await this.dataTable
                    .Where(x =>
                        x.RefUserId == user.Id
                        && !x.Deleted
                        && ((x.ReportStartDate.Year == year && x.ReportStartDate.Month == month) || (x.ReportEndDate.Year == year && x.ReportEndDate.Month == month))
                    ).OrderBy(x => x.ReportStartDate).ThenBy(x => x.ReportEndDate).ThenBy(x => x.ReportStartTime).ThenBy(x => x.ReportEndTime)
                    .ToEnumerableAsync();

                //Fix issue#2 - BEGIN
                ObservableCollection<ReportItem> reports = new ObservableCollection<ReportItem>();
                foreach (var item in items)
                {
                    item.ReportEndDate = DateTime.SpecifyKind
                        (
                            item.ReportEndDate.Date.Add(item.ReportEndTime),
                            DateTimeKind.Utc
                        ).ToLocalTime();
                    item.ReportStartDate = DateTime.SpecifyKind
                        (
                            item.ReportStartDate.Date.Add(item.ReportStartTime),
                            DateTimeKind.Utc
                        ).ToLocalTime();
                    item.ReportEndTime = item.ReportEndDate.TimeOfDay;
                    item.ReportStartTime = item.ReportStartDate.TimeOfDay;

                    reports.Add(item);
                }
                //Fix issue#2 - END

                return reports;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"-------------------[Debug] ReportManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"-------------------[Debug] ReportManager Sync error: " + e.Message);
            }
            return null;
        }

        //Fix issue#2 - BEGIN
        public new async Task SaveTaskAsync(ReportItem report)
        {
            await BaseAuthProvider.RefreshProfile();
            report.ReportEndDate
                = report.ReportEndDate.Date.Add(report.ReportEndTime).ToUniversalTime();
            report.ReportStartDate
                = report.ReportStartDate.Date.Add(report.ReportStartTime).ToUniversalTime();
            report.ReportEndTime = report.ReportEndDate.TimeOfDay;
            report.ReportStartTime = report.ReportStartDate.TimeOfDay;

            await base.SaveTaskAsync(report);
        }

        public new async Task<ReportItem> LookupAsync(string id)
        {
            await BaseAuthProvider.RefreshProfile();
            ReportItem report = await base.LookupAsync(id);

            report.ReportEndDate = DateTime.SpecifyKind
                (
                    report.ReportEndDate.Date.Add(report.ReportEndTime),
                    DateTimeKind.Utc
                ).ToLocalTime();
            report.ReportStartDate = DateTime.SpecifyKind
                (
                    report.ReportStartDate.Date.Add(report.ReportStartTime),
                    DateTimeKind.Utc
                ).ToLocalTime();
            report.ReportEndTime = report.ReportEndDate.TimeOfDay;
            report.ReportStartTime = report.ReportStartDate.TimeOfDay;

            return report;
        }
        //Fix issue#2 - END
    }
}
