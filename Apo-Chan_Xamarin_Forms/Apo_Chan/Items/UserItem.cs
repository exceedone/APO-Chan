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
        /// <summary>
        /// 
        /// </summary>
        /// <returns>If Non-Cached -- null else UserItem object.</returns>
        public static UserItem GetCachedUserItem()
        {
            if (!Application.Current.Properties.Any())
            {
                return null;
            }
            var json = Application.Current.Properties.GetOrDefault("UserInfo") as string;
            try
            {
                UserItem user = JsonConvert.DeserializeObject<UserItem>(json);
                user.AccessToken = Convert.ToString(Application.Current.Properties.GetOrDefault("AccessToken"));
                user.RefreshToken = Convert.ToString(Application.Current.Properties.GetOrDefault("RefreshToken"));
                user.AMSToken = Convert.ToString(Application.Current.Properties.GetOrDefault("AMSToken"));
                string d = Convert.ToString(Application.Current.Properties.GetOrDefault("ExpiresOn"));
                if (!string.IsNullOrWhiteSpace(d))
                {
                    user.ExpiresOn = Convert.ToDateTime(d);
                }
                return user;
            }
            catch
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
            Application.Current.Properties.Add("UserInfo", Newtonsoft.Json.Linq.JObject.FromObject(this).ToString());
            // not send API properties
            Application.Current.Properties.AddOrSkip("AccessToken", this.AccessToken);
            Application.Current.Properties.AddOrSkip("RefreshToken", this.RefreshToken);
            Application.Current.Properties.AddOrSkip("AMSToken", this.AMSToken);
            if (this.ExpiresOn.HasValue)
            {
                Application.Current.Properties.AddOrSkip("ExpiresOn", this.ExpiresOn.Value.ToString());
            }
            // await Application.Current.SavePropertiesAsync();
        }

        public static async void ClearUserToken()
        {
            Application.Current.Properties.Clear();
            // await Application.Current.SavePropertiesAsync();
        }
    }

}
