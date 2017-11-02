using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.WindowsAzure.MobileServices;

namespace Apo_Chan.Managers
{
    public partial class GroupManager : BaseManager<GroupItem>
    {
        //public override string SyncQueryName
        //{
        //    get { return "allReportItems"; }
        //}

        static GroupManager()
        {
            defaultInstance = new GroupManager();
        }

        public GroupManager() : base() { }

        public static GroupManager DefaultManager
        {
            get
            {
                return defaultInstance as GroupManager;
            }
            private set
            {
                defaultInstance = value;
            }
        }
        public async Task<ObservableCollection<GroupItem>> GetItemsAsync(Expression<Func<GroupItem, bool>> expression, bool syncItems = false)
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

                IEnumerable<GroupItem> items = await this.dataTable
                    .Where(x =>
                         !x.Deleted
                    )
                    .Where(expression)
                    .ToEnumerableAsync();

                ObservableCollection<GroupItem> groups = new ObservableCollection<GroupItem>();
                foreach (var item in items)
                {
                    groups.Add(item);
                }

                return groups;
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

        public new async Task SaveTaskAsync(GroupItem group)
        {
            if (string.IsNullOrWhiteSpace(group.GroupKey))
            {
                group.GroupKey = System.Guid.NewGuid().ToString().Replace("-", "");
            }
            await BaseAuthProvider.RefreshProfile();
            await base.SaveTaskAsync(group);
        }

        public new async Task<GroupItem> LookupAsync(string id)
        {
            await BaseAuthProvider.RefreshProfile();
            GroupItem group = await base.LookupAsync(id);

            return group;
        }
    }
}
