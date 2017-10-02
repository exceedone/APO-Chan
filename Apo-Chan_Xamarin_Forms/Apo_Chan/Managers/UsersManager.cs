using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            // get from Azure Mobile Apps
            try
            {
                // not token update info
                //await BaseAuthProvider.RefreshProfile();
                IEnumerable<UserItem> items = await this.dataTable
                    .Where(x => x.UserProviderId == providerId)
                    .ToEnumerableAsync();

                if (!items.Any()) { return null; }
                return items.ToList().FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"-------------------[Debug] UsersManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"-------------------[Debug] UsersManager Sync error: " + e.Message);
            }
            return null;
        }

    }
}
