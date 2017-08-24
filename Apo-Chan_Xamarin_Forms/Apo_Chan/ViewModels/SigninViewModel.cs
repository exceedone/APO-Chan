using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Apo_Chan.ViewModels
{
    public class SignInViewModel : BindableBase
    {
        private bool authenticated;
        private INavigationService navigationService;

        public DelegateCommand NavigateSignInCommand { get; private set; }
        public DelegateCommand NavigateSignInGoogleCommand { get; private set; }
        public DelegateCommand NavigateSignOutCommand { get; private set; }

        //constructor
        public SignInViewModel(INavigationService navigationService)
        {
            this.authenticated = false;
            this.navigationService = navigationService;
            NavigateSignInCommand = new DelegateCommand(NavigateSignIn);
            NavigateSignInGoogleCommand = new DelegateCommand(NavigateSignInGoogle);
            NavigateSignOutCommand = new DelegateCommand(NavigateSignOut);

        }

        private async void NavigateSignIn()
        {
            if (App.Authenticator != null)
                authenticated = await App.Authenticator.AuthenticateAsync(Constants.EProviderType.Office365);

            // Set syncItems to true to synchronize the data on startup when offline is enabled.
            if (authenticated == true)
            {
                await navigationService.NavigateAsync("UserReportList");
            }
        }
        private async void NavigateSignInGoogle()
        {
            if (App.Authenticator != null)
                authenticated = await App.Authenticator.AuthenticateAsync(Constants.EProviderType.Google);

            // Set syncItems to true to synchronize the data on startup when offline is enabled.
            if (authenticated == true)
            {
                await navigationService.NavigateAsync("UserReportList");
            }
        }
        private async void NavigateSignOut()
        {
            if (App.Authenticator != null)
            {
                await App.Authenticator.SignOutAsync();
            }
            //// Set syncItems to true to synchronize the data on startup when offline is enabled.
            //if (authenticated == true)
            //{
            //    await navigationService.NavigateAsync("SignIn");
            //}
        }



    }
}
