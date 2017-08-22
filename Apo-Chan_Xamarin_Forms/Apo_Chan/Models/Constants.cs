using System;

namespace Apo_Chan
{
	public static class Constants
	{
		// Replace strings with your Azure Mobile App endpoint.
		public static string ApplicationURL = @"https://apo-chandev.azurewebsites.net";

        /// <summary>
        /// Application name.
        /// </summary>
        public static string ApplicationName = "Apo-Chan";


        /// <summary>
        /// Provider Type
        /// </summary>
        public enum EProviderType
        {
            Google = 1
            , Microsoft = 2
            , Office365 = 3
        }
    }
}

