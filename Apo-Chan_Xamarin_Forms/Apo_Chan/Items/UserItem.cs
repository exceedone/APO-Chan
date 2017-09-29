using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;
using Xamarin.Auth;
using Xamarin.Forms;
using Apo_Chan.Models;
using Apo_Chan.Managers;
using Newtonsoft.Json.Linq;

namespace Apo_Chan.Items
{
    [DataContract(Name = "user")]
    public class UserItem : BaseItem
    {
        [JsonProperty(PropertyName = "providerType")]
        public int ProviderType { get; set; }

        [JsonProperty(PropertyName = "userProviderId")]
        public string UserProviderId { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Provider Service Token(Not Coneect Mobile App)
        /// </summary>
        //[JsonProperty(PropertyName = "AccessToken")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Provider Service Token(Not Coneect Mobile App)
        /// </summary>
        //[JsonProperty(PropertyName = "RefreshToken")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Azure Mobile Service Token(Not Coneect Mobile App)
        /// </summary>
        //[JsonProperty(PropertyName = "AMSToken")]
        public string AMSToken { get; set; }

        /// <summary>
        /// Mobile service User ID
        /// </summary>
        public string AMSUserId { get; set; }
        /// <summary>
        /// Token Expires DateTime
        /// </summary>
        public DateTime? ExpiresOn { get; set; }

        public Constants.EProviderType EProviderType
        {
            get
            {
                return (Constants.EProviderType)Enum.Parse(typeof(Constants.EProviderType), this.ProviderType.ToString());
            }
        }

        public MobileServiceUser MobileServiceUser
        {
            get
            {
                return new MobileServiceUser(this.AMSUserId) { MobileServiceAuthenticationToken = this.AMSToken };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>If Non-Cached -- null else UserItem object.</returns>
        public static UserItem GetCachedUserItem()
        {
            try
            {
                var account = AccountStore.Create().FindAccountsForService(Constants.ApplicationName).FirstOrDefault();
                if (account != null)
                {
                    UserItem user = JsonConvert.DeserializeObject<UserItem>(account.Properties["UserInfo"]);
                    user.AccessToken = account.Properties["AccessToken"];
                    user.RefreshToken = account.Properties["RefreshToken"];
                    user.AMSToken = account.Properties["AMSToken"];
                    user.AMSUserId = account.Properties["AMSUserId"];
                    string d = account.Properties["ExpiresOn"];
                    if (!string.IsNullOrWhiteSpace(d))
                    {
                        user.ExpiresOn = Convert.ToDateTime(d);
                    }
                    return user;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save userinfo
        /// (1) Get UserTable Id by UserProviderId and ProviderType
        /// (2) If (1) is null, insert new data
        /// (3) cashe
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="token"></param>
        public async Task SetUserToken()
        {
            if (this != null && string.IsNullOrWhiteSpace(this.Id))
            {
                // (1)
                UserItem item = null;
                try
                {
                    // get this id.
                    item = await UsersManager.DefaultManager.GetItemAsync(this.UserProviderId, this.ProviderType);
                }
                catch
                { }
                // (2)if item is null(first access), insert this item
                if (item == null)
                {
                    try
                    {
                        await UsersManager.DefaultManager.SaveTaskAsync(this);
                    }
                    catch (Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException mex)
                    { }
                    catch (Exception ex)
                    { }
                }
                else
                {
                    this.Id = item.Id;
                }
            }

            // (3) set cached
            ClearUserToken();

            Account account = new Account
            {
                Username = this.AMSUserId
            };
            account.Properties.Add("UserInfo", JObject.FromObject(this).ToString());
            account.Properties.Add("AccessToken", this.AccessToken);
            account.Properties.Add("RefreshToken", this.RefreshToken);
            account.Properties.Add("AMSToken", this.AMSToken);
            account.Properties.Add("AMSUserId", this.AMSUserId);
            if (this.ExpiresOn.HasValue)
            {
                account.Properties.Add("ExpiresOn", this.ExpiresOn.Value.ToString());
            }
            AccountStore.Create().Save(account, Constants.ApplicationName);
        }

        public static void ClearUserToken()
        {
            var accounts = AccountStore.Create().FindAccountsForService(Constants.ApplicationName);
            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    AccountStore.Create().Delete(account, Constants.ApplicationName);
                }
            }
        }

    }

}
