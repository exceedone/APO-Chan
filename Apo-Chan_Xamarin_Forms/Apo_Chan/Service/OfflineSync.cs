using Apo_Chan.Items;
using Apo_Chan.Managers;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace Apo_Chan.Service
{
    public static class OfflineSync
    {
        private const string OfflineDbPath = @"localdb.sqlite";

        private static OfflineSyncResult syncResult;
        public static OfflineSyncResult SyncResult
        {
            get
            {
                if (syncResult == null)
                {
                    syncResult = new OfflineSyncResult();
                }
                return syncResult;
            }
        }

        public static void InitOfflineSyncContext()
        {
            var store = new MobileServiceSQLiteStore(OfflineDbPath);

            store.DefineTable<UserItem>();
            store.DefineTable<ReportItem>();
            store.DefineTable<GroupItem>();
            store.DefineTable<GroupUserItem>();
            store.DefineTable<ReportGroupItem>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            if (!App.CurrentClient.SyncContext.IsInitialized)
            {
                InitLocalStore(store);
            }
        }

        private static async void InitLocalStore(MobileServiceSQLiteStore store)
        {
            Stopwatch diff = new Stopwatch();
            diff.Start();
            await App.CurrentClient.SyncContext.InitializeAsync(store);
            diff.Stop();
            Models.DebugUtil.WriteLine("OfflineSync InitLocalStore: " + diff.Elapsed);
        }

        public static async Task PerformAlInOneSync()
        {
            while (!App.CurrentClient.SyncContext.IsInitialized)
            {
                Models.DebugUtil.WriteLine($"OfflineSync PerformAlInOneSync wait initialize ... ");
                await Task.Delay(100);
            }

            Models.DebugUtil.WriteLine($"OfflineSync PerformAlInOneSync Pending Push Sync: {SyncResult.PendingSyncItems}");

            SyncResult.Reset();

            Stopwatch diff = new Stopwatch();
            diff.Start();
            await PushAsync();
            await GroupUserManager.DefaultManager.SyncAsync();
            await ReportGroupManager.DefaultManager.SyncAsync();
            await UsersManager.DefaultManager.SyncAsync();
            await ReportManager.DefaultManager.SyncAsync();
            await GroupManager.DefaultManager.SyncAsync();
            diff.Stop();

            Models.DebugUtil.WriteLine("OfflineSync SyncedItems: " + SyncResult.SyncedItems);
            foreach (var item in SyncResult.OfflineSyncErrors)
            {
                Models.DebugUtil.WriteLine($"OfflineSync Error: {item.Item1} {item.Item2}");
            }

            Models.DebugUtil.WriteLine($"OfflineSync PerformAlInOneSync elapsed: {diff.Elapsed} pending: {SyncResult.PendingSyncItems}");
        }

        private static async Task PushAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await App.CurrentClient.SyncContext.PushAsync();
                SyncResult.SyncedItems++;
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
                SyncResult.OfflineSyncErrors.Add(Tuple.Create("PushAsync", 2));
            }
            catch (Exception e)
            {
                Models.DebugUtil.WriteLine($"OfflineSync PushAsync error: {e.Message}");
                SyncResult.OfflineSyncErrors.Add(Tuple.Create("PushAsync", 1));
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
                    Models.DebugUtil.WriteLine($"OfflineSync PushAsync error Item: {error.TableName} ({error.Item["id"]}). Operation discarded.");
                }
            }
        }

        public class OfflineSyncResult
        {
            public long PendingSyncItems
            {
                get
                {
                    return App.CurrentClient.SyncContext.PendingOperations;
                }
            }
            public int SyncedItems { get; set; }
            public List<Tuple<string, int>> OfflineSyncErrors { get; set; }

            public void Reset()
            {
                SyncedItems = 0;

                if (OfflineSyncErrors == null)
                {
                    OfflineSyncErrors = new List<Tuple<string, int>>();
                }
                else
                {
                    OfflineSyncErrors.Clear();
                }
            }
        }
    }
}
