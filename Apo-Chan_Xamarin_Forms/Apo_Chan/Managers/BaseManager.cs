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
            IMobileServiceTableQuery<T1> query = this.localDataTable.CreateQuery();
            try
            {
                await this.localDataTable.PullAsync(this.SyncQueryName, query);
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
