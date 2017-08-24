using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;

namespace Apo_Chan.Models
{
    public static class Extension
    {
        public static MobileServiceAuthenticationProvider MobileServiceAuthenticationProvider(this Constants.EProviderType providerType)
        {
            switch (providerType)
            {
                case Constants.EProviderType.Google:
                    return Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.Google;
                case Constants.EProviderType.Microsoft:
                    return Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.MicrosoftAccount;
                case Constants.EProviderType.Office365:
                    return Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory;
            }
            return Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory;
        }

        /// <summary>
        /// Dictionary Add or skip
        /// key is null・・・skip
        /// value is null・・・skip
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrSkip<T1, T2>(this IDictionary<T1, T2> dict, T1 key, T2 value)
        {
            if (key == null) { return; }
            if (value == null) { return; }
            dict.Add(key, value);
        }

        public static T2 GetOrDefault<T1, T2>(this IDictionary<T1, T2> dict, T1 key)
        {
            if (!dict.ContainsKey(key)) { return default(T2); }
            return dict[key];
        }
    }
}
