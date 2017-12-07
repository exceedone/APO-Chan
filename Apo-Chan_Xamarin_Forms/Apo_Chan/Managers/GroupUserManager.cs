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
                    query = query.Where(x => x.RefGroupId == refGroupId);
                }
                if (!string.IsNullOrWhiteSpace(refUserId))
                {
                    query = query.Where(x => x.RefUserId == refUserId);
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

        public async Task DeleteAsync(string id)
        {
            await BaseAuthProvider.RefreshProfile();
            GroupUserItem groupuser = await base.LookupAsync(id);

            await localDataTable.DeleteAsync(groupuser);
        }

        public override async Task SyncAsync()
        {
            IMobileServiceTableQuery<GroupUserItem> query;
            try
            {
                //pull groups that user belong to
                query = localDataTable.Where(x => x.RefUserId == GlobalAttributes.User.Id);

                await this.localDataTable.PullAsync(this.SyncQueryName, query);

                ObservableCollection<string> groupList = await GetGroupIdList();

                query = localDataTable.Where(x => groupList.Contains(x.RefGroupId));
                await this.localDataTable.PullAsync(this.SyncQueryName + " list", query);

                Service.OfflineSync.SyncResult.SyncedItems++;
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine($"{this.SyncQueryName} Manager PullAsync error: " + e.Message);
                Service.OfflineSync.SyncResult.OfflineSyncErrors.Add(Tuple.Create(SyncQueryName, 1));
            }
        }


        //sync support
        public async Task<ObservableCollection<string>> GetGroupIdList()
        {
            ObservableCollection<string> groupList = new ObservableCollection<string>();

            IEnumerable<GroupUserItem> items = await this.localDataTable
                    .Where(x => !x.Deleted)
                    .OrderBy(x => x.RefGroupId)
                    .ToEnumerableAsync();
            GroupUserItem prevGroup = new GroupUserItem
            {
                RefGroupId = string.Empty,
                RefUserId = string.Empty
            };
            foreach (var item in items)
            {
                if (item.RefGroupId != prevGroup.RefGroupId)
                {
                    groupList.Add(item.RefGroupId);
                    prevGroup = item;
                }
            }

            return groupList;
        }

        //sync support
        public async Task<ObservableCollection<string>> GetUserIdList()
        {
            ObservableCollection<string> userList = new ObservableCollection<string>();

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
                    userList.Add(item.RefUserId);
                    prevUser = item;
                }
            }

            return userList;
        }

        //Client implementation of "api/values/userjoingroups/{userid}"
        public async Task<ObservableCollection<GroupAndUserCountItem>> GetGroupAndUserCountList(string userId)
        {
            ObservableCollection<GroupAndUserCountItem> groupCountList = new ObservableCollection<GroupAndUserCountItem>();

            ObservableCollection<string> groupList;
            groupList = await this.GetGroupIdList();

            foreach (var groupId in groupList)
            {
                var group = await GroupManager.DefaultManager.GetItemAsync(groupId);
                var groupUserCount = await this.localDataTable.Where(x => x.RefGroupId == groupId).ToEnumerableAsync();

                GroupAndUserCountItem item = new GroupAndUserCountItem();
                item.Group = group;
                item.UserCount = groupUserCount.Count();
                item.AdminFlg = group.CreatedUserId.CompareTo(userId) == 0;

                groupCountList.Add(item);
            }

            return groupCountList;
        }

        //Client implementation of "table/groupuser/list/{groupid}"
        public async Task UpsertGroup(string groupId, ObservableCollection<GroupUserItem> groupUserItems)
        {
            ObservableCollection<GroupUserItem> existedGroup = await this.GetItemsAsync(refGroupId: groupId);

            List<string> existedGroupUserId = new List<string>();

            foreach (var item in groupUserItems)
            {
                var i = existedGroup.FirstOrDefault(x => x.RefUserId == item.RefUserId);
                if (i == null)
                {
                    await SaveTaskAsync(item);
                }
                else
                {
                    existedGroupUserId.Add(i.Id);
                }
            }

            foreach (var gu in existedGroup)
            {
                if (!existedGroupUserId.Contains(gu.Id))
                {
                    await DeleteAsync(gu.Id);
                }
            }
        }

        //Client implementation of "api/values/groupusers/{groupid}"
        public async Task<GroupAndGroupUsersItem> GetGroupAndGroupUsers(string groupId)
        {
            GroupAndGroupUsersItem groupAndGroupUsers = new GroupAndGroupUsersItem();
            var group = await GroupManager.DefaultManager.GetItemAsync(groupId);

            ObservableCollection<GroupUserItem> groupUsers = await this.GetItemsAsync(refGroupId: groupId);

            if (group == null || groupUsers == null)
            {
                return null;
            }

            foreach (var item in groupUsers)
            {
                item.RefGroup = await GroupManager.DefaultManager.GetItemAsync(item.RefGroupId);
                item.RefUser = await UsersManager.DefaultManager.GetItemAsync(x => x.Id == item.RefUserId);
            }

            groupAndGroupUsers.Group = group;
            groupAndGroupUsers.GroupUsers = groupUsers.ToList();

            return groupAndGroupUsers;
        }

        public async Task DeleteGroupAsync(GroupItem group)
        {
            ObservableCollection<GroupUserItem> existedGroup = await this.GetItemsAsync(refGroupId: group.Id);

            foreach (var item in existedGroup)
            {
                await DeleteAsync(item.Id);
            }
        }
    }
}
