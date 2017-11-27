using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Apo_Chan.Items;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace Apo_Chan.Managers
{
    /// <summary>
    /// Base Manager Class
    /// </summary>
    public abstract partial class BaseManager<T1> where T1 : BaseItem
    {
        protected static BaseManager<T1> defaultInstance;

#if OFFLINE_SYNC_ENABLED
        protected IMobileServiceSyncTable<T1> dataTable;
#else
        protected IMobileServiceTable<T1> dataTable;
#endif

        public BaseManager()
        {

#if OFFLINE_SYNC_ENABLED
            this.dataTable = App.CurrentClient.GetSyncTable<T1>();
#else
            this.dataTable = App.CurrentClient.GetTable<T1>();
#endif
        }

        public bool IsOfflineEnabled
        {
            get { return dataTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<T1>; }
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
                await dataTable.InsertAsync(item);
            }
            else
            {
                await dataTable.UpdateAsync(item);
            }
        }

        public async Task DeleteAsync(T1 item)
        {
            await dataTable.DeleteAsync(item);
        }

        public async Task<T1> LookupAsync(string id)
        {
            return await dataTable.LookupAsync(id);
        }

#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync(bool pushFirst = true)
        {
            while (!App.CurrentClient.SyncContext.IsInitialized)
            {
                Debug.WriteLine($"-------------------[Debug] BaseManager<{SyncQueryName}> SyncAsync wait initialize ... ");
                await Task.Delay(100);
            }
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            if (pushFirst)
            {
                try
                {
                    await App.CurrentClient.SyncContext.PushAsync();
                }
                catch (MobileServicePushFailedException exc)
                {
                    if (exc.PushResult != null)
                    {
                        syncErrors = exc.PushResult.Errors;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"-------------------[Debug] BaseManager<{SyncQueryName}> PushAsync error: " + e.Message);
                }

                // Simple error/conflict handling. A real application would handle the various errors like network conditions,
                // server conflicts and others via the IMobileServiceSyncHandler.
                if (syncErrors != null)
                {
                    foreach (var error in syncErrors)
                    {
                        if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                        {
                            //Update failed, reverting to server's copy.
                            await error.CancelAndUpdateItemAsync(error.Result);
                        }
                        else
                        {
                            // Discard local change.
                            await error.CancelAndDiscardItemAsync();
                        }
                        Debug.WriteLine($"-------------------[Debug] BaseManager<{SyncQueryName}>" +
                            $" PushAsync error. Item: {error.TableName} ({error.Item["id"]}). Operation discarded.");
                    }
                }
            }

            try
            {
                await this.dataTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    this.SyncQueryName,
                    this.dataTable.CreateQuery());
                }
            catch (Exception e)
            {
                Debug.WriteLine($"-------------------[Debug] BaseManager<{SyncQueryName}> PullAsync error: " + e.Message);
            }
        }
#endif
    }
}
