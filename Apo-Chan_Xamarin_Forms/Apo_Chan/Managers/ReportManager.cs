using Apo_Chan.Items;
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
                // Get UserItem
                UserItem user = UserItem.GetCachedUserItem();
                IEnumerable<ReportItem> items = await this.dataTable
                    .Where(x => 
                        x.RefUserId == user.Id 
                        && !x.Deleted 
                        && ((x.ReportStartDate.Year == year && x.ReportStartDate.Month == month) || (x.ReportEndDate.Year == year && x.ReportEndDate.Month == month))
                    ).OrderBy(x => x.ReportStartDate).ThenBy(x => x.ReportEndDate).ThenBy(x => x.ReportStartTime).ThenBy(x => x.ReportEndTime)
                    .ToEnumerableAsync();

                return new ObservableCollection<ReportItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"-------------------[Debug] Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"-------------------[Debug] Sync error: " + e.Message);
            }
            return null;
        }

#if TEST_LOCAL
        public async Task<ObservableCollection<ReportItem>> GetItemsAsync(int year, int month)
        {
            return await Test.TestReportLocalStore.GetItems(year, month);
        }
        public new async Task SaveTaskAsync(ReportItem item)
        {
            if (item.Id == null)
            {
                await Test.TestReportLocalStore.InsertItem(item);
            }
            else
            {
                await Test.TestReportLocalStore.UpdateItem(item);
            }
        }

        public new async Task DeleteAsync(ReportItem item)
        {
            await Test.TestReportLocalStore.DeleteItem(item);
        }

        public new async Task<ReportItem> LookupAsync(string id)
        {
            return await Test.TestReportLocalStore.GetItem(id);
        }
#endif
    }
}
