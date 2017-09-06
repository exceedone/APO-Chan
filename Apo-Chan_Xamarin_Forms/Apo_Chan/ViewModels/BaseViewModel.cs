using Prism.Commands;
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

            this.SettingCommand = new DelegateCommand(setting);
            this.GoBackCommand = new DelegateCommand(executeGoBack);
        }
        #endregion


        #region Function
        private async void setting()
        {
            await this.navigationService.NavigateAsync("Setting");
        }
        private async void executeGoBack()
        {
            await navigationService.GoBackAsync();
        }
        #endregion
    }
}
