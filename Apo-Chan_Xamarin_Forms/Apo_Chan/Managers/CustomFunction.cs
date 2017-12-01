using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Flurl;
using Flurl.Util;

namespace Apo_Chan.Managers
{
    public class CustomFunction
    {
        /// <summary>
        /// Execute Custom Web API from Mobile Service
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<T> Get<T>(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.ApplicationURL);
                var user = GlobalAttributes.User;
                if (user != null)
                {
                    client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", user.AMSToken);
                }
                // ADD required querystring "ZUMO-API-VERSION=2.0.0"
                var requri = uri.SetQueryParam("ZUMO-API-VERSION", "2.0.0");

                var response = await client.GetAsync(requri);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return (JsonConvert.DeserializeObject<T>(json));
                }
                else
                {
                    Models.DebugUtil.WriteLine($"Error : {Constants.ApplicationURL}{uri} Message:"
                        + await response.RequestMessage.Content.ReadAsStringAsync());
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Execute Custom Web API from Mobile Service
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Post<T>(string uri, T data) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.ApplicationURL);
                var user = GlobalAttributes.User;
                if (user != null)
                {
                    client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", user.AMSToken);
                }
                // ADD required querystring "ZUMO-API-VERSION=2.0.0"
                var requri = uri.SetQueryParam("ZUMO-API-VERSION", "2.0.0");
                
                // Serialize our concrete class into a JSON String
                string stringPayload = JsonConvert.SerializeObject(data);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requri, httpContent);
                return response;
            }
        }
    }
}
