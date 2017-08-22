/*
 * 
 */
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
        public override async Task<ObservableCollection<ReportItem>> GetItemsAsync(bool syncItems = false)
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

                IEnumerable<ReportItem> items = await this.dataTable
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
    }
}
