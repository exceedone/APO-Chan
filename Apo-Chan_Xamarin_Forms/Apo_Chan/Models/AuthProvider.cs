using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Apo_Chan.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            //client.BaseAddress = new Uri( Constants.ApplicationURL + "/.auth/me"); // todo:Flurl(porable)
            client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", amsAccessToken);
            var response = await client.GetStringAsync(Constants.ApplicationURL + "/.auth/me");
            return response;
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

            foreach (var claim in jobj.user_claims)
            {
                if (claim.typ == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                {
                    user.UserProviderId = claim.val;
                }
            }
        }

        public class ProviderProfileObj
        {
            public string user_id { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public List<ProviderProfileClaimObj> user_claims { get; set; }

            public class ProviderProfileClaimObj
            {
                public string typ { get; set; }
                public string val { get; set; }
            }
        }
    }

    public class ProviderProfileObj
    {
        public string user_id { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public List<ProviderProfileClaimObj> user_claims { get; set; }

        public class ProviderProfileClaimObj
        {
            public string typ { get; set; }
            public string val { get; set; }
        }
    }
}
