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
using System.IO;

namespace Apo_Chan.Items
{
    [DataContract(Name = "user")]
    public class UserItem : BaseItem
    {
        [JsonProperty(PropertyName = "providerType")]
        public int ProviderType { get; set; }

        [JsonProperty(PropertyName = "userProviderId")]
        public string UserProviderId { get; set; }

        private string userName;
        [JsonProperty(PropertyName = "userName")]
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                SetProperty(ref this.userName, value);
            }
        }


        private string email;
        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get
            {
                return this.email;
            }
            set
            {
                SetProperty(ref this.email, value);
            }
        }

        /// <summary>
        /// Provider Service Token(Not Coneect Mobile App)
        /// </summary>
        //[JsonProperty(PropertyName = "AccessToken")]
        [JsonIgnore]
        public string AccessToken { get; set; }

        /// <summary>
        /// Provider Service Token(Not Coneect Mobile App)
        /// </summary>
        //[JsonProperty(PropertyName = "RefreshToken")]
        [JsonIgnore]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Azure Mobile Service Token(Not Coneect Mobile App)
        /// </summary>
        //[JsonProperty(PropertyName = "AMSToken")]
        [JsonIgnore]
        public string AMSToken { get; set; }

        /// <summary>
        /// Mobile service User ID
        /// </summary>
        [JsonIgnore]
        public string AMSUserId { get; set; }

        //private byte[] userImageByte;
        ///// <summary>
        ///// User Image(base64)
        ///// </summary>
        //[JsonIgnore]
        //public byte[] UserImageByte
        //{
        //    get
        //    {
        //        // for first time, getting from Azure Blob
        //        if (userImageByte == null)
        //        {
        //            try
        //            {
        //                this.userImageByte = Task.Run(() => Service.ImageService.DownloadFile(this)).Result;
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }
        //        return this.userImageByte;
        //    }
        //    set
        //    {
        //        this.userImageByte = value;
        //    }
        //}

        [JsonIgnore]
        private CustomImageSource userImage;
        /// <summary>
        /// User Image
        /// </summary>
        [JsonIgnore]
        public CustomImageSource UserImage
        {
            get
            {
                if (this.userImage == null)
                {
                    return CustomImageSource.FromFile(Constants.IconAccountName);
                }
                return userImage;
            }
            set
            {
                SetProperty(ref this.userImage, value);
            }
        }

        /// <summary>
        /// Token Expires DateTime
        /// </summary>
        [JsonIgnore]
        public DateTime? ExpiresOn { get; set; }

        [JsonIgnore]
        public Constants.EProviderType EProviderType
        {
            get
            {
                return (Constants.EProviderType)Enum.Parse(typeof(Constants.EProviderType), this.ProviderType.ToString());
            }
        }

        [JsonIgnore]
        public MobileServiceUser MobileServiceUser
        {
            get
            {
                return new MobileServiceUser(this.AMSUserId) { MobileServiceAuthenticationToken = this.AMSToken };
            }
        }

        [JsonIgnore]
        public string NameAndEmail
        {
            get
            {
                return $"{this.UserName}({this.Email})";
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
                    user.AccessToken = account.Properties.GetOrDefault("AccessToken");
                    user.RefreshToken = account.Properties.GetOrDefault("RefreshToken");
                    user.AMSToken = account.Properties.GetOrDefault("AMSToken");
                    user.AMSUserId = account.Properties.GetOrDefault("AMSUserId");
                    string d = account.Properties.GetOrDefault("ExpiresOn");
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
                    //item = await UsersManager.DefaultManager.GetItemAsync(this.UserProviderId, this.ProviderType);
                    // getting user info from server
                    item = await UsersManager.DefaultManager.GetItemAsync(this.UserProviderId);
                }
                catch (Exception ex)
                {
                    DebugUtil.WriteLine("UserItem SetUserToken err: " + ex.Message);
                }
                // (2)if item is null(first access), insert this item
                if (item == null)
                {
                    try
                    {
                        await UsersManager.DefaultManager.SaveTaskAsync(this);
                    }
                    catch (MobileServiceInvalidOperationException mex)
                    {
                        Stream receiveStream = await mex.Response.Content.ReadAsStreamAsync();
                        StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                        string error = readStream.ReadToEnd();

                    }
                    catch (Exception)
                    {

                    }
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
            account.Properties.AddOrSkip("UserInfo", JObject.FromObject(this).ToString());
            string terst = JObject.FromObject(this).ToString();
            account.Properties.AddOrSkip("AccessToken", this.AccessToken);
            account.Properties.AddOrSkip("RefreshToken", this.RefreshToken);
            account.Properties.AddOrSkip("AMSToken", this.AMSToken);
            account.Properties.AddOrSkip("AMSUserId", this.AMSUserId);
            if (this.ExpiresOn.HasValue)
            {
                account.Properties.AddOrSkip("ExpiresOn", this.ExpiresOn.Value.ToString());
            }
            AccountStore.Create().Save(account, Constants.ApplicationName);
        }

        /// <summary>
        /// Delete User Info from AccountStore
        /// </summary>
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
