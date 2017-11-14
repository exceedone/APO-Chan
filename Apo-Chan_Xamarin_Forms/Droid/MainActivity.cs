using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Json;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;

using Apo_Chan.Managers;
using Apo_Chan.Items;
using Apo_Chan.Models;
using Plugin.Permissions;
using HockeyApp.Android;

[assembly: Dependency(typeof(Apo_Chan.Droid.MainActivity))]

namespace Apo_Chan.Droid
{
    [Activity(Label = "APO-Chan",
        Icon = "@drawable/ic_launcher",
        //MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait,
        Theme = "@style/MyTheme")]
    public class MainActivity : FormsAppCompatActivity, IAuthenticate
    {
        // Define a authenticated user.
        private MobileServiceUser loginuser;

        protected override void OnCreate(Bundle bundle)
        {
            ToolbarResource = Resource.Layout.Toolbar;
            TabLayoutResource = Resource.Layout.Tabbar;

            base.OnCreate(bundle);

            // Initialize Azure Mobile Apps
            CurrentPlatform.Init();

            //Xamarin Forms 2.4.x
            //Forms.SetFlags("FastRenderers_Experimental");
            // Initialize Xamarin Forms
            Forms.Init(this, bundle);

            // Initialize the authenticator before loading the app.
            App.Init((IAuthenticate)this);

            // Init CircleImage
            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();

            // Load the main application
            LoadApplication(new App());
        }
        protected override void OnResume()
        {
            base.OnResume();
        }

        public async Task<bool> AuthenticateAsync(Constants.EProviderType providerType)
        {
            var success = false;
            try
            {
                // Sign in using a server-managed flow.
                loginuser = await App.CurrentClient.LoginAsync
                    (
                        this,
                        providerType.MobileServiceAuthenticationProvider(),
                        "apochan-scheme",
                        providerType.MobileServiceParameters()
                    );
                if (loginuser != null)
                {
                    UserItem user = new UserItem()
                    {
                        AMSToken = loginuser.MobileServiceAuthenticationToken
                        ,
                        AMSUserId = loginuser.UserId
                        ,
                        ProviderType = providerType.GetHashCode()
                    };

                    // get provider user profile.
                    BaseAuthProvider providerObj = BaseAuthProvider.GetAuthProvider(providerType);
                    string json = await providerObj.GetProfileJson(loginuser.MobileServiceAuthenticationToken);
                    providerObj.SetUserProfile(user, json);
                    
                    success = true;
                    await user.SetUserToken();
                    await providerObj.GetUserPicture(user);
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

        async Task<bool> IAuthenticate.SignOutAsync()
        {
            var auth = BaseAuthProvider.GetAuthProvider(GlobalAttributes.User.EProviderType);
            await App.CurrentClient.LogoutAsync();
            if (!string.IsNullOrWhiteSpace(auth.GetSignoutUrl()))
            {
                BrowserService.OpenUrl(this, auth.GetSignoutUrl());
            }
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
