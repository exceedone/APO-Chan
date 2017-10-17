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
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", amsAccessToken);
                var response = await client.GetStringAsync(Constants.ApplicationURL + "/.auth/me");
                return response;
            }
        }

        /// <summary>
        /// Refresh AMS token, Access token if past expires_on
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RefreshProfile()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var user = GlobalAttributes.User;

                    // if past expires_on, refresh token
                    if (!user.ExpiresOn.HasValue || user.ExpiresOn.Value < DateTime.Now)
                    {
                        client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", user.AMSToken);
                        var response = await client.GetStringAsync(Constants.ApplicationURL + "/.auth/refresh");

                        JObject jObject = JObject.Parse(response);
                        user.AMSToken = Convert.ToString(jObject["authenticationToken"]);
                        App.CurrentClient.CurrentUser.MobileServiceAuthenticationToken = user.AMSToken;

                        BaseAuthProvider provider = BaseAuthProvider.GetAuthProvider(user.EProviderType);
                        string json = await provider.GetProfileJson(user.AMSToken);
                        provider.SetUserProfile(user, json);
                        await user.SetUserToken();
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// First Process
        /// </summary>
        /// <returns>true: has userinfo false: none userinfo</returns>
        public static bool FirstProcess()
        {
            // First, get user info.
            UserItem user = GlobalAttributes.User;
            // If user info is null, return false.
            if (user == null)
            {
                return false;
            }
            App.CurrentClient.CurrentUser = user.MobileServiceUser;
            return true;
        }


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

        /// <summary>
        /// Set User Info(Email, name, userid)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="json"></param>
        public abstract void SetUserProfile(UserItem user, string json);

        public abstract Task GetUserPicture(UserItem user);
        public abstract string GetSignoutUrl();

    }


    public class Google : BaseAuthProvider
    {
        /// <summary>
        /// Get User Profile Picture And POST to Azure Blob
        /// </summary>
        /// <param name="user"></param>
        public override async Task GetUserPicture(UserItem user)
        {
            //try
            //{
            //    string pictureUrl = null;
            //    using (HttpClient client = new HttpClient())
            //    {
            //        var response = await client.GetAsync(string.Format(Constants.GoogleApisURI, "userinfo", user.AccessToken));
            //        if (response.IsSuccessStatusCode)
            //        {
            //            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            //            pictureUrl = Convert.ToString(json["picture"]);
            //        }
            //    }
            //    if (!string.IsNullOrWhiteSpace(pictureUrl))
            //    {
            //        using (HttpClient client = new HttpClient())
            //        {
            //            var response = await client.GetAsync(pictureUrl);
            //            if (response.IsSuccessStatusCode)
            //            {
            //                using (var stream = await response.Content.ReadAsStreamAsync())
            //                {
            //                    // upload file to Azure(not wait.)
            //                    AzureBlobAPI.UploadFile(user, stream);
            //                    user.UserImageBase64 = Utils.Base64FromStream(stream);
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

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
        public override string GetSignoutUrl()
        {
            return null;
            //return new Uri($"https://accounts.google.com/Logout?continue=https://appengine.google.com/_ah/logout?continue={Constants.ApplicationURL}/api/values/signout%3fZUMO-API-VERSION=2.0.0");
            //return new Uri($"https://www.google.com/accounts/Logout?continue={Uri.EscapeDataString(Constants.ApplicationURL + "/api/values/signout?ZUMO-API-VERSION=2.0.0")}");
        }
    }

    public class ActiveDirectory : BaseAuthProvider
    {
        /// <summary>
        /// Get User Profile Picture And POST to Azure Blob
        /// </summary>
        /// <param name="user"></param>
        public override async Task GetUserPicture(UserItem user)
        {
            //try
            //{
            //    using (HttpClient client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
            //        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //        var response = await client.GetAsync($"{Constants.MicrosoftGraphApiURI}/me/photo/%24value");// %24 → $
            //        if (response.IsSuccessStatusCode)
            //        {
            //            using (var stream = await response.Content.ReadAsStreamAsync())
            //            {
            //                // upload file to Azure(not wait.)
            //                AzureBlobAPI.UploadFile(user, stream);
            //                user.UserImageBase64 = Utils.Base64FromStream(stream);
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public override void SetUserProfile(UserItem user, string json)
        {
            ProviderProfileObj jobj = JsonConvert.DeserializeObject<List<ProviderProfileObj>>(json).FirstOrDefault();
            user.Email = jobj.user_id;
            user.AccessToken = jobj.access_token;
            user.RefreshToken = jobj.refresh_token;

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
                else if (claim.typ == "exp")
                {
                    user.ExpiresOn = UnixTime.FromUnixTime(Convert.ToInt64(claim.val));// claim.val;
                }
            }
        }

        public override string GetSignoutUrl()
        {
            return Constants.MicrosoftSignoutApiURI;
        }
    }

    public class ProviderProfileObj
    {
        public string user_id { get; set; }
        public string access_token { get; set; }
        public string id_token { get; set; }
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
