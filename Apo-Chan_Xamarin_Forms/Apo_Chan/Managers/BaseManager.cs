using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Apo_Chan.Items;
using Apo_Chan.Service;

namespace Apo_Chan.Managers
{
    /// <summary>
    /// Base Manager Class
    /// </summary>
    public abstract partial class BaseManager<T1> where T1 : BaseItem
    {
        protected static BaseManager<T1> defaultInstance;

        protected IMobileServiceSyncTable<T1> localDataTable;
        protected IMobileServiceTable<T1> remoteDataTable;

        public BaseManager()
        {

            this.localDataTable = App.CurrentClient.GetSyncTable<T1>();
            //this.remoteDataTable = App.CurrentClient.GetTable<T1>();
        }

        public string SyncQueryName
        {
            get
            {
                return typeof(T1).Name;
            }
        }
        public async Task SaveTaskAsync(T1 item)
        {
            if (item.Id == null)
            {
                await localDataTable.InsertAsync(item);
            }
            else
            {
                await localDataTable.UpdateAsync(item);
            }
        }

        public async Task DeleteAsync(T1 item)
        {
            await localDataTable.DeleteAsync(item);
        }

        public async Task<T1> LookupAsync(string id)
        {
            return await localDataTable.LookupAsync(id);
        }

        public virtual async Task SyncAsync()
        {
            IMobileServiceTableQuery<T1> query0;
            try
            {
                switch (this.SyncQueryName)
                {
                    case "UserItem":
                        {
                            IMobileServiceTableQuery<UserItem> query;
                            query = (localDataTable as IMobileServiceSyncTable<UserItem>)
                                .Where(x => x.UserProviderId == GlobalAttributes.User.UserProviderId);
                        }
                        
                        break;
                    case "ReportItem":
                        {
                            IMobileServiceTableQuery<ReportItem> query;
                            query = (localDataTable as IMobileServiceSyncTable<ReportItem>)
                                .Where(x => x.RefUserId == GlobalAttributes.User.Id);
                        }
                        
                        break;
                    case "GroupItem":
                        {
                            IMobileServiceTableQuery<GroupItem> query;
                            query = (localDataTable as IMobileServiceSyncTable<GroupItem>)
                                .Where(x => x.CreatedUserId == GlobalAttributes.User.Id);
                        }
                        break;
                    case "GroupUserItem":
                        break;
                    case "ReportGroupItem":
                        break;
                    default:
                        query0 = localDataTable.CreateQuery();
                        break;
                }
                await this.localDataTable.PullAsync(
                    this.SyncQueryName,
                    this.localDataTable.CreateQuery());
                OfflineSync.SyncResult.SyncedItems++;
            }
            catch (Exception e)
            {
                Models.DebugUtil.WriteLine($"BaseManager<{SyncQueryName}> PullAsync error: " + e.Message);
                OfflineSync.SyncResult.OfflineSyncErrors.Add(Tuple.Create(SyncQueryName, 1));
            }
        }
    }
}
