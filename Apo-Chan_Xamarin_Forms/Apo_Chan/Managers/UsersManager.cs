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
                IEnumerable<UserItem> items = await this.dataTable
                    .Where(x => x.UserProviderId == providerId)
                    .ToEnumerableAsync();

                if (!items.Any()) { return null; }
                return items.ToList().FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"-------------------[Debug] Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"-------------------[Debug] Sync error: " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// Save userinfo
        /// (1) Get UserTable Id by UserProviderId and ProviderType
        /// (2) If (1) is null, insert new data
        /// (3) cashe
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="token"></param>
        public static async Task CacheUserToken(UserItem user)
        {
            if (user != null && string.IsNullOrWhiteSpace(user.Id))
            {
                // (1)
                UserItem item = null;
                try
                {
                    // get user id.
                    item = await DefaultManager.GetItemAsync(user.UserProviderId, user.ProviderType);
                }
                catch
                { }
                // (2)if item is null(first access), insert user item
                if (item == null)
                {
                    try
                    {
                        await UsersManager.DefaultManager.SaveTaskAsync(user);
                    }
                    catch (Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException mex)
                    { }
                    catch (Exception ex)
                    { }
                }
                else
                {
                    user.Id = item.Id;
                }
            }

            // (3) set cached
            Application.Current.Properties.Clear();
            Application.Current.Properties.Add("UserInfo", Newtonsoft.Json.Linq.JObject.FromObject(user).ToString());
            // not send API properties
            Application.Current.Properties.AddOrSkip("AccessToken", user.AccessToken);
            Application.Current.Properties.AddOrSkip("RefreshToken", user.RefreshToken);
            Application.Current.Properties.AddOrSkip("AMSToken", user.AMSToken);

        }

    }
}
