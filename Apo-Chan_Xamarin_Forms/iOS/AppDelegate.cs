using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Foundation;
using UIKit;

using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using Apo_Chan.Models;
using Apo_Chan.Items;

namespace Apo_Chan.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate, IAuthenticate
    {
        // Define a authenticated user.
        private MobileServiceUser loginuser;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Initialize Azure Mobile Apps
            CurrentPlatform.Init();

            // Initialize Xamarin Forms
            Forms.Init();

            // Init CircleImage
            ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();

            App.Init(this);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public async Task<bool> AuthenticateAsync(Constants.EProviderType providerType)
        {
            var success = false;
            try
            {
                // Sign in using a server-managed flow.
                loginuser = await App.CurrentClient.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, providerType.MobileServiceAuthenticationProvider(), "apochan-scheme");
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
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                // Display the success or failure message.
                UIAlertView avAlert = new UIAlertView("Sign-in result", message, null, "OK", null);
                avAlert.Show();
            }

            return success;
        }

        public async Task<bool> SignOutAsync()
        {
            await App.CurrentClient.LogoutAsync();
            return true;
        }

        /// <summary>
        /// Override OpenUrl Function
        /// </summary>
        /// <param name="app"></param>
        /// <param name="url"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return App.CurrentClient.ResumeWithURL(url);
        }
    }
}

