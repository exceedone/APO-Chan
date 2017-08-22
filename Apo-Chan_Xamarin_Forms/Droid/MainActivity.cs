using System;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;

using Apo_Chan.Managers;
using Apo_Chan.Items;

namespace Apo_Chan.Droid
{
	[Activity (Label = "Apo-Chan",
		Icon = "@drawable/icon",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		Theme = "@android:style/Theme.Holo.Light")]
	public class MainActivity : FormsApplicationActivity, IAuthenticate
    {
        // Define a authenticated user.
        private MobileServiceUser user;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Initialize Azure Mobile Apps
			CurrentPlatform.Init();

			// Initialize Xamarin Forms
			Forms.Init (this, bundle);

            // Initialize the authenticator before loading the app.
            App.Init((IAuthenticate)this);

            // Load the main application
            LoadApplication (new App ());
		}
        
        public async Task<bool> AuthenticateAsync(Constants.EProviderType providerType)
        {
            var success = false;
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                user = await UsersManager.DefaultManager.CurrentClient.LoginAsync(this,
                    MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory, "apochan-scheme");
                if (user != null)
                {
                    success = true;
                    await UsersManager.CacheUserToken(user.UserId, user.MobileServiceAuthenticationToken, providerType);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                // Display the success or failure message.
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage(message);
                builder.SetTitle("Sign-in result");
                builder.Create().Show();
            }

            return success;
        }
    }
}

