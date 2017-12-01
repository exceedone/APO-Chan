using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Apo_Chan.Managers
{
    public partial class GroupUserManager : BaseManager<GroupUserItem>
    {
        private ObservableCollection<GroupUserItem> groupList;
        public ObservableCollection<GroupUserItem> GroupList
        {
            get
            {
                if (groupList == null)
                {
                    groupList = new ObservableCollection<GroupUserItem>();
                }
                return groupList;
            }
        }
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
        public async Task<ObservableCollection<GroupUserItem>> GetItemsAsync(string refGroupId = null, string refUserId = null)
        {
            // get from Azure Mobile Apps
            try
            {
                await BaseAuthProvider.RefreshProfile();
                var query = this.localDataTable.Where(x => !x.Deleted);
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
                DebugUtil.WriteLine("GroupUserManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine("GroupUserManager Sync error: " + e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<GroupUserItem>> GetItemsAsync(Expression<Func<GroupUserItem, bool>> extension)
        {
            // get from Azure Mobile Apps
            try
            {
                await BaseAuthProvider.RefreshProfile();
                IEnumerable<GroupUserItem> items = await this.localDataTable
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
                DebugUtil.WriteLine("GroupUserManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine("GroupUserManager Sync error: " + e.Message);
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

        public override async Task SyncAsync()
        {
            IMobileServiceTableQuery<GroupUserItem> query;
            try
            {
                //pull groups that user belong to
                query = localDataTable.Where(x => x.RefUserId == GlobalAttributes.User.Id);

                await this.localDataTable.PullAsync(this.SyncQueryName, query);

                //pull users that are in groups with user
                IEnumerable<GroupUserItem> items = await this.localDataTable
                    .Where(x => !x.Deleted)
                    .ToEnumerableAsync();

                GroupList.Clear();
                foreach (var item in items)
                {
                    query = localDataTable.Where(x => x.RefGroupId == item.RefGroupId);
                    await this.localDataTable.PullAsync(this.SyncQueryName + item.RefGroupId, query);

                    GroupList.Add(item);
                }

                Service.OfflineSync.SyncResult.SyncedItems++;
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine($"{this.SyncQueryName} Manager PullAsync error: " + e.Message);
                Service.OfflineSync.SyncResult.OfflineSyncErrors.Add(Tuple.Create(SyncQueryName, 1));
            }
        }

        public async Task<ObservableCollection<GroupUserItem>> GetUserList()
        {
            ObservableCollection<GroupUserItem> userList = new ObservableCollection<GroupUserItem>();

            IEnumerable<GroupUserItem> items = await this.localDataTable
                    .Where(x => !x.Deleted)
                    .OrderBy(x => x.RefUserId)
                    .ToEnumerableAsync();
            GroupUserItem prevUser = new GroupUserItem
            {
                RefGroupId = string.Empty,
                RefUserId = string.Empty
            };
            foreach (var item in items)
            {
                if (item.RefUserId != prevUser.RefUserId)
                {
                    userList.Add(item);
                    prevUser = item;
                }
            }

            return userList;
        }
    }
}
