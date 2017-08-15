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
    public partial class ReportManager : BaseManager<Report>
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
        public override async Task<ObservableCollection<Report>> GetItemsAsync(bool syncItems = false)
        {
            //var reportStore = new ObservableCollection<Report>
            //{
            //    new Report()
            //    {
            //        Id = "123", RefUserId = "asd",
            //        ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(18,38,30),
            //        ReportTitle = "report 1"
            //    },
            //    new Report()
            //    {
            //        Id = "124", RefUserId = "asd",
            //        ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(18,40,0),
            //        ReportTitle = "report 2"
            //    },
            //    new Report()
            //    {
            //        Id = "125", RefUserId = "asd",
            //        ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(19,45,30),
            //        ReportTitle = "report 3"
            //    },
            //    new Report()
            //    {
            //        Id = "126", RefUserId = "asd",
            //        ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(20,30,0),
            //        ReportTitle = "report 4"
            //    },
            //};
            //return reportStore;

            // get from Azure Mobile Apps
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

                IEnumerable<Report> items = await this.dataTable
                    .ToEnumerableAsync();

                return new ObservableCollection<Report>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }
    }
}
