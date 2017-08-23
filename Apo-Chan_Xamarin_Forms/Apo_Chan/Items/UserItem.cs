using System;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;
using Xamarin.Auth;
using Xamarin.Forms;
using Apo_Chan.Models;

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
        /// 
        /// </summary>
        /// <returns>If Non-Cached -- null else UserItem object.</returns>
        public static UserItem GetCachedUserItem()
        {
            
            var json = Application.Current.Properties.GetOrDefault("UserInfo") as string;
            UserItem user = JsonConvert.DeserializeObject<UserItem>(json);
            user.AccessToken = Convert.ToString(Application.Current.Properties.GetOrDefault("AccessToken"));
            user.RefreshToken = Convert.ToString(Application.Current.Properties.GetOrDefault("RefreshToken"));
            user.AMSToken = Convert.ToString(Application.Current.Properties.GetOrDefault("AMSToken"));

            return user;
        }
    }

}
