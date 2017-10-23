using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;
using System.Linq.Expressions;

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

namespace System
{
    public static class Ex
    {
        /// <summary>
        /// compare two array.
        /// first & second = null → true
        /// first or second = null → false
        /// first and second != null → compare
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool EqualArray<TSource>(this IList<TSource> first, IList<TSource> second)
        {
            if(first == null && second == null) { return true; }
            if (first == null || second == null) { return false; }

            if(first.Count != second.Count)
            {
                return false;
            }
            for (int i = 0; i < first.Count; i++)
            {
                if(!first[i].Equals(second[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}