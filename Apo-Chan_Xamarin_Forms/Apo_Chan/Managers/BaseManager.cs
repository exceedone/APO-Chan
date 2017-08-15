using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Apo_Chan.Items;

namespace Apo_Chan.Managers
{
    /// <summary>
    /// Base Manager Class
    /// </summary>
    public abstract partial class BaseManager<T1> where T1 : BaseItem
    {
        protected static BaseManager<T1> defaultInstance;
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        protected IMobileServiceSyncTable<T1> dataTable;
#else
        protected IMobileServiceTable<T1> dataTable;
#endif

        const string offlineDbPath = @"localstore.db";

        public BaseManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<T>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.dataTable = client.GetSyncTable<Table>();
#else
            this.dataTable = client.GetTable<T1>();
#endif
        }

        //public static BaseManager<T1> DefaultManager
        //{
        //    get
        //    {
        //        return defaultInstance;
        //    }
        //    private set
        //    {
        //        defaultInstance = value;
        //    }
        //}

        public MobileServiceClient CurrentClient
        {
            get { return client; }
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
        public abstract Task<ObservableCollection<T1>> GetItemsAsync(bool syncItems = false);

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

#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.dataTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    this.SyncQueryName,
                    this.dataTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
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

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif
    }
}
