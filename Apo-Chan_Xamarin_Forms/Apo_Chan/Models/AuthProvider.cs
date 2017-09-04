using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using Apo_Chan.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Apo_Chan.Managers;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Auth;

namespace Apo_Chan.Models
{
    /// <summary>
    /// Authentication provider
    /// </summary>
    public abstract class BaseAuthProvider
    {
        /// <summary>
        /// GetProfileJson from AMS
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetProfileJson(string amsAccessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", amsAccessToken);
            var response = await client.GetStringAsync(Constants.ApplicationURL + "/.auth/me");
            return response;
        }

        /// <summary>
        /// First Process
        /// </summary>
        /// <returns>true: has userinfo false: none userinfo</returns>
        public async static Task<bool> FirstProcess()
        {
            // First, get user info.
            UserItem user = UserItem.GetCachedUserItem();
            // If user info is null, return false.
            if (user == null)
            {
                return false;
            }

            try
            {
                // login using access token
                //JObject jo = new JObject();
                //jo.Add("access_token", user.AMSToken);
                //var loginuser = await App.CurrentClient.LoginAsync(user.EProviderType.MobileServiceAuthenticationProvider(), jo);

                // if past expires_on, refresh token
                //if (!user.ExpiresOn.HasValue || user.ExpiresOn.Value < DateTime.Now)
                //{
                //    var client = new HttpClient();
                //    client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", user.AMSToken);
                //    var response = await client.GetStringAsync(Constants.ApplicationURL + "/.auth/refresh");
                //    AuthenticationObj mobileServiceUser = JsonConvert.DeserializeObject<AuthenticationObj>(response);
                //    user.AMSToken = mobileServiceUser.authenticationToken;
                //    BaseAuthProvider provider = BaseAuthProvider.GetAuthProvider(user.EProviderType);
                //    string json = await provider.GetProfileJson(user.AMSToken);
                //    provider.SetUserProfile(user, json);
                //    await user.SetUserToken();
                //}

                // get provider user profile.
                //BaseAuthProvider providerObj = BaseAuthProvider.GetAuthProvider((Constants.EProviderType)Enum.Parse(typeof(Constants.EProviderType), user.ProviderType.ToString()));
                //string json = await providerObj.GetProfileJson(user.AMSToken);
                //providerObj.SetUserProfile(user, json);

                //await UserItem.SetUserToken(user);
                return true;
                // TODO: if can get loginuser info
            }
            catch (Exception ex)
            {
                // if we cannot get userinfo, clear cache.
                Debug.WriteLine(@"-------------------[Debug] AccessToken error: " + ex.Message);
                UserItem.ClearUserToken();
                return false;
            }
        }


        /// <summary>
        /// Set User Info(Email, name, userid)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="json"></param>
        public abstract void SetUserProfile(UserItem user, string json);

        public static BaseAuthProvider GetAuthProvider(Constants.EProviderType providerType)
        {
            switch (providerType)
            {
                case Constants.EProviderType.Google:
                    return new Google();
                case Constants.EProviderType.Office365:
                    return new ActiveDirectory();
            }
            return new ActiveDirectory();
        }
    }


    public class Google : BaseAuthProvider
    {
        public override void SetUserProfile(UserItem user, string json)
        {
            ProviderProfileObj jobj = JsonConvert.DeserializeObject<List<ProviderProfileObj>>(json).FirstOrDefault();
            user.Email = jobj.user_id;
            user.AccessToken = jobj.access_token;
            user.RefreshToken = jobj.refresh_token;
            if (jobj.expires_on.HasValue)
            {
                user.ExpiresOn = jobj.expires_on.Value.ToLocalTime();
            }

            foreach (var claim in jobj.user_claims)
            {
                if (claim.typ == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                {
                    user.UserProviderId = claim.val;
                }
                else if (claim.typ == "name")
                {
                    user.UserName = claim.val;
                }
            }
        }

    }

    public class ActiveDirectory : BaseAuthProvider
    {
        public override void SetUserProfile(UserItem user, string json)
        {
            ProviderProfileObj jobj = JsonConvert.DeserializeObject<List<ProviderProfileObj>>(json).FirstOrDefault();
            user.Email = jobj.user_id;
            user.AccessToken = jobj.access_token;
            user.RefreshToken = jobj.refresh_token;
            if (jobj.expires_on.HasValue)
            {
                user.ExpiresOn = jobj.expires_on.Value.ToLocalTime();
            }

            foreach (var claim in jobj.user_claims)
            {
                if (claim.typ == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                {
                    user.UserProviderId = claim.val;
                }
                else if (claim.typ == "name")
                {
                    user.UserName = claim.val;
                }
            }
        }
    }

    public class ProviderProfileObj
    {
        public string user_id { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public DateTime? expires_on { get; set; }
        public List<ProviderProfileClaimObj> user_claims { get; set; }

        public class ProviderProfileClaimObj
        {
            public string typ { get; set; }
            public string val { get; set; }
        }
    }
    public class AuthenticationObj
    {
        public string authenticationToken { get; set; }
    }
}
