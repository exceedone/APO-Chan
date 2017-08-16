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
            var reportStore = new ObservableCollection<ReportItem>
            {
                new ReportItem()
                {
                    Id = "123", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(18,38,30),
                    ReportTitle = "Meeting with company ABC"
                },
                new ReportItem()
                {
                    Id = "124", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(18,40,0),
                    ReportTitle = "Meeting with company DEF"
                },
                new ReportItem()
                {
                    Id = "125", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(19,45,30),
                    ReportTitle = "Develop at Home"
                },
                new ReportItem()
                {
                    Id = "126", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(20,30,0),
                    ReportTitle = "Cafe Develop"
                },
                new ReportItem()
                {
                    Id = "127", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,15), ReportStartTime = new TimeSpan(09,00,0),
                    ReportTitle = "Writing technical blog"
                },
                new ReportItem()
                {
                    Id = "128", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,16), ReportStartTime = new TimeSpan(14,00,0),
                    ReportTitle = "Push GitHub"
                },
                new ReportItem()
                {
                    Id = "129", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,19), ReportStartTime = new TimeSpan(16,30,0),
                    ReportTitle = "Discuss next features"
                },
                new ReportItem()
                {
                    Id = "130", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,21), ReportStartTime = new TimeSpan(11,30,0),
                    ReportTitle = "Tracking project progress"
                },
            };
            return reportStore;

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
