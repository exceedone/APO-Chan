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
    public partial class GroupUserManager : BaseManager<GroupUserItem>
    {
        //public override string SyncQueryName
        //{
        //    get { return "allReportItems"; }
        //}

        static GroupUserManager()
        {
            defaultInstance = new GroupUserManager();
        }

        public GroupUserManager() : base() { }

        public static GroupUserManager DefaultManager
        {
            get
            {
                return defaultInstance as GroupUserManager;
            }
            private set
            {
                defaultInstance = value;
            }
        }
        public async Task<ObservableCollection<GroupUserItem>> GetItemsAsync(string refGroupId = null, string refUserId = null, bool syncItems = false)
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
                var query = this.dataTable.Where(x => !x.Deleted);
                if (!string.IsNullOrWhiteSpace(refGroupId))
                {
                    query.Where(x => x.RefGroupId == refGroupId);
                }
                if (!string.IsNullOrWhiteSpace(refUserId))
                {
                    query.Where(x => x.RefUserId == refUserId);
                }
                IEnumerable<GroupUserItem> items = await query.ToEnumerableAsync();

                ObservableCollection<GroupUserItem> groups = new ObservableCollection<GroupUserItem>();
                foreach (var item in items)
                {
                    groups.Add(item);
                }

                return groups;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"-------------------[Debug] GroupUserManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"-------------------[Debug] GroupUserManager Sync error: " + e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<GroupUserItem>> GetItemsAsync(Expression<Func<GroupUserItem, bool>> extension, bool syncItems = false)
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
                IEnumerable<GroupUserItem> items = await this.dataTable
                    .Where(x => !x.Deleted)
                    .Where(extension)
                    .ToEnumerableAsync();

                ObservableCollection<GroupUserItem> groups = new ObservableCollection<GroupUserItem>();
                foreach (var item in items)
                {
                    groups.Add(item);
                }

                return groups;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"-------------------[Debug] GroupUserManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"-------------------[Debug] GroupUserManager Sync error: " + e.Message);
            }
            return null;
        }

        public new async Task SaveTaskAsync(GroupUserItem groupuser)
        {
            await BaseAuthProvider.RefreshProfile();
            await base.SaveTaskAsync(groupuser);
        }

        public new async Task<GroupUserItem> LookupAsync(string id)
        {
            await BaseAuthProvider.RefreshProfile();
            GroupUserItem group = await base.LookupAsync(id);

            return group;
        }
    }
}
