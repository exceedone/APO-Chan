using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Apo_Chan.ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        #region Variable and Property
        private bool authenticated;

        public DelegateCommand NavigateSignInCommand { get; private set; }
        public DelegateCommand NavigateSignInGoogleCommand { get; private set; }
        #endregion

        #region Constructor
        public SignInViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            this.authenticated = false;
            this.navigationService = navigationService;
            NavigateSignInCommand = new DelegateCommand(NavigateSignIn);
            NavigateSignInGoogleCommand = new DelegateCommand(NavigateSignInGoogle);
        }
        #endregion

        #region Function
        private async void NavigateSignIn()
        {
            if (!GlobalAttributes.IsConnectedInternet)
            {
                await dialogService.DisplayAlertAsync("Error", "APO-Chan cannot connect to the Internet!", "OK");
                return;
            }
            this.IsBusy = true;
            if (App.Authenticator != null)
                authenticated = await App.Authenticator.AuthenticateAsync(Constants.EProviderType.Office365);

            // Set syncItems to true to synchronize the data on startup when offline is enabled.
            if (authenticated == true)
            {
                await navigationService.NavigateAsync("/NavigationPage/UserReportList");
            }
            this.IsBusy = false;
        }
        private async void NavigateSignInGoogle()
        {
            if (!GlobalAttributes.IsConnectedInternet)
            {
                await dialogService.DisplayAlertAsync("Error", "APO-Chan cannot connect to the Internet!", "OK");
                return;
            }
            this.IsBusy = true;
            if (App.Authenticator != null)
                authenticated = await App.Authenticator.AuthenticateAsync(Constants.EProviderType.Google);

            // Set syncItems to true to synchronize the data on startup when offline is enabled.
            if (authenticated == true)
            {
                await navigationService.NavigateAsync("/NavigationPage/UserReportList");
            }
            this.IsBusy = false;
        }
        #endregion

    }
}
