using Apo_Chan.Items;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Apo_Chan.Service
{
    public static class OfflineSync
    {
        private const string OfflineDbPath = @"localdb.sqlite";


        public static void InitOfflineSyncContext()
        {
            var store = new MobileServiceSQLiteStore(OfflineDbPath);
            store.DefineTable<UserItem>();
            store.DefineTable<ReportItem>();
            //store.DefineTable<GroupItem>();
            //store.DefineTable<GroupUserItem>();
            //store.DefineTable<ReportGroupItem>();
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
            Debug.WriteLine(@"-------------------[Debug] OfflineSync InitLocalStore: " + diff.Elapsed);
        }
    }
}
