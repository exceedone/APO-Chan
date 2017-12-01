using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        public async Task<ObservableCollection<GroupItem>> GetItemsAsync(Expression<Func<GroupItem, bool>> expression)
        {
            // get from Azure Mobile Apps
            try
            {
                await BaseAuthProvider.RefreshProfile();
                var user = GlobalAttributes.User;

                IEnumerable<GroupItem> items = await this.localDataTable
                    //.Where(x =>
                    //     !x.Deleted
                    //)
                    .Where(expression)
                    .ToEnumerableAsync();

                ObservableCollection<GroupItem> groups = new ObservableCollection<GroupItem>();
                foreach (var item in items)
                {
                    if (!item.Deleted)
                    {
                        groups.Add(item);
                    }
                }

                return groups;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                DebugUtil.WriteLine("GroupManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine("GroupManager Sync error: " + e.Message);
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

        public override async Task SyncAsync()
        {
            try
            {
                foreach (var item in GroupUserManager.DefaultManager.GroupList)
                {
                    //pull groups by id
                    var query = localDataTable.Where(x => x.Id == item.RefGroupId);
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
    }
}
