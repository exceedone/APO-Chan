using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Apo_Chan.Managers
{
    /// <summary>
    /// "UserManager" is same name in  namespace "Android.OS".
    /// So rename User"s"Manager.
    /// </summary>
    public partial class UsersManager : BaseManager<UserItem>
    {
        static UsersManager()
        {
            defaultInstance = new UsersManager();
        }
        public UsersManager() : base() { }

        public static UsersManager DefaultManager
        {
            get
            {
                return defaultInstance as UsersManager;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public async Task<UserItem> GetItemAsync(string providerId, int providerType)
        {
            return await this.GetItemAsync(x => x.UserProviderId == providerId);
        }

        public async Task<UserItem> GetItemAsync(Expression<Func<UserItem, bool>> expression)
        {
            // get from Azure Mobile Apps
            try
            {
                // not token update info
                //await BaseAuthProvider.RefreshProfile();
                IEnumerable<UserItem> items = await this.localDataTable
                    .Where(expression)
                    .ToEnumerableAsync();

                if (!items.Any()) { return null; }
                return items.ToList().FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                DebugUtil.WriteLine("UsersManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine("UsersManager Sync error: " + e.Message);
            }
            return null;
        }

        //Online only
        public async Task<UserItem> GetItemAsync(string providerId)
        {
            try
            {
                this.remoteDataTable = App.CurrentClient.GetTable<UserItem>();

                IEnumerable<UserItem> items = await remoteDataTable
                    .Where(x => x.UserProviderId == providerId)
                    .ToEnumerableAsync();

                if (!items.Any()) { return null; }
                return items.ToList().FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                DebugUtil.WriteLine("UsersManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                DebugUtil.WriteLine("UsersManager Sync error: " + e.Message);
            }
            return null;
        }

        public override async Task SyncAsync()
        {
            IMobileServiceTableQuery<UserItem> query;
            try
            {
                query = localDataTable.Where(x => x.UserProviderId == GlobalAttributes.User.UserProviderId);

                await this.localDataTable.PullAsync(this.SyncQueryName, query);
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
