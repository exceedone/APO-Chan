using System;
using System.Threading.Tasks;
using Apo_Chan.Geolocation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace Apo_Chan.ViewModels
{
    public abstract class BaseViewModel : BindableBase
    {
        protected INavigationService navigationService;

        protected IPageDialogService dialogService;

        public DelegateCommand SettingCommand { get; private set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                SetProperty(ref this.isBusy, value);
            }
        }

        public DelegateCommand GoBackCommand { get; private set; }

        #region Constructor
        public BaseViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            this.navigationService = navigationService;
            this.dialogService = dialogService;

            this.SettingCommand = new DelegateCommand(navigateSetting);
            this.GoBackCommand = new DelegateCommand(executeGoBack);
        }
        #endregion


        #region Function
        private async void navigateSetting()
        {
            await this.navigationService.NavigateAsync("Setting");
        }
        private async void executeGoBack()
        {
            await navigationService.GoBackAsync();
        }

        protected async void InitLocationServiceAsync()
        {
            System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + "initLocationServiceAsync()");
            await GeoService.DefaultInstance.GetPositionAsync(alertGeoServiceStatusAsync);
        }

        private async Task alertGeoServiceStatusAsync(string message)
        {
            await dialogService.DisplayAlertAsync("Location", message, "OK");
        }
        #endregion
    }
}
