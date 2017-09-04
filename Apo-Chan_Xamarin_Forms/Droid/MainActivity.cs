﻿using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Json;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;

using Apo_Chan.Managers;
using Apo_Chan.Items;
using Apo_Chan.Models;

namespace Apo_Chan.Droid
{
    [Activity(Label = "Apo-Chan",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Theme = "@android:style/Theme.Holo.Light")]
    public class MainActivity : FormsApplicationActivity, IAuthenticate
    {
        // Define a authenticated user.
        private MobileServiceUser loginuser;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Initialize Azure Mobile Apps
            CurrentPlatform.Init();

            // Initialize Xamarin Forms
            Forms.Init(this, bundle);

            // Initialize the authenticator before loading the app.
            App.Init((IAuthenticate)this);

            // Load the main application
            LoadApplication(new App());
        }

        public async Task<bool> AuthenticateAsync(Constants.EProviderType providerType)
        {
            var success = false;
            try
            {
                // Sign in using a server-managed flow.
                loginuser = await App.CurrentClient.LoginAsync(this, providerType.MobileServiceAuthenticationProvider(), "apochan-scheme");
                if (loginuser != null)
                {
                    UserItem user = new UserItem()
                    {
                        AMSToken = loginuser.MobileServiceAuthenticationToken
                        ,
                        ProviderType = providerType.GetHashCode()
                    };

                    // get provider user profile.
                    BaseAuthProvider providerObj = BaseAuthProvider.GetAuthProvider(providerType);
                    string json = await providerObj.GetProfileJson(loginuser.MobileServiceAuthenticationToken);
                    providerObj.SetUserProfile(user, json);

                    success = true;
                    await user.SetUserToken();
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
            await App.CurrentClient.LogoutAsync();
            return true;
        }

    }
}

