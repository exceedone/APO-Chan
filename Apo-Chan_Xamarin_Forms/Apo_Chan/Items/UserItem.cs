using System;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;
using Xamarin.Auth;
using Apo_Chan.Managers;

namespace Apo_Chan.Items
{
    [DataContract(Name = "user")]
    public class UserItem : BaseItem
    {
        [JsonProperty(PropertyName = "providerType")]
        public Constants.EProviderType? ProviderType { get; set; }

        [JsonProperty(PropertyName = "userProviderId")]
        public string UserProviderId { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "deletedAt")]
        public DateTime DeletedAt { get; set; }

        /// <summary>
        /// Token(Not Coneect Mobile App)
        /// </summary>
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>If Non-Cached -- null else UserItem object.</returns>
        public static UserItem GetCachedUserItem()
        {
            var account = AccountStore.Create().FindAccountsForService(Constants.ApplicationName).FirstOrDefault();
            if (account == null)
            {
                return null;
            }

            var json = account.Properties["UserInfo"].ToString();
            UserItem user = JsonConvert.DeserializeObject<UserItem>(json);
            
            return user;
        }
    }

}
