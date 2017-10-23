using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Apo_Chan.Items;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
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
        public static async Task<T> Get<T>(string uri) where T:class
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.ApplicationURL);
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
                    Debug.WriteLine($"-------------------[Debug] Error : {Constants.ApplicationURL}{uri} Message:" + await response.RequestMessage.Content.ReadAsStringAsync());
                    return null;
                }
            }
        }
    }
}
