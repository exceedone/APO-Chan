using System;
using System.Collections.Generic;
using System.Linq;
using Apo_Chan.Models;

namespace Apo_Chan
{
    public static class Constants
    {
        // Replace strings with your Azure Mobile App endpoint.
        public static string ApplicationURL = @"https://apo-chandev.azurewebsites.net";

        /// <summary>
        /// Application name.
        /// </summary>
        public static string ApplicationName = "APO-Chan";

        public static string IconAccountName = "icon_account.png";

        /// <summary>
        /// Provider Type
        /// </summary>
        public enum EProviderType
        {
            Google = 1
            , Microsoft = 2
            , Office365 = 3
        }

        /// <summary>
        /// Google Maps Geocoding API - REST services
        /// </summary>
        public static string GoogleReverseGeocodingURI = "https://maps.googleapis.com/maps/api/geocode/json?latlng={1},{2}&key={0}";

        /// <summary>
        /// Google Apis
        /// </summary>
        public static string GoogleApisURI = "https://www.googleapis.com/oauth2/v1/{0}?alt=json&access_token={1}";

        /// <summary>
        /// MicrosoftGraphApi
        /// </summary>
        public static string MicrosoftGraphApiURI = "https://graph.microsoft.com/v1.0";

        /// <summary>
        /// MicrosoftSignoutApi
        /// </summary>
        public static string MicrosoftSignoutApiURI = "https://login.microsoftonline.com/common/oauth2/logout";

        /// <summary>
        /// Google Maps Geocoding API Key
        /// </summary>
        public static string GoogleGeocodingAPIKey = "AIzaSyCxHoYGMlu-y0BvLFHaoO14zD0E3eWxbVQ";

        /// <summary>
        /// GitHub URL(APO-Chan)
        /// </summary>
        public static string GitHubURL = "https://github.com/exceedone/APO-Chan";


        /// <summary>
        /// ExceedOne Company URL
        /// </summary>
        public static string ExcedOneURL = "http://www.exceedone.co.jp/";

        public static IEnumerable<AuthModel> AuthPicker
        {
            get
            {
                return new[]{
                    new AuthModel(){ AdminFlg = false, Label = "User"}
                    , new AuthModel(){ AdminFlg = true, Label = "Admin"}
            };
            }
        }
    }
}

