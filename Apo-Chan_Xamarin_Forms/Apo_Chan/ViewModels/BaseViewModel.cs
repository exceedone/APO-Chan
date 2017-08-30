using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Commands;

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


        #region Constructor
        public BaseViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            this.navigationService = navigationService;
            this.dialogService = dialogService;

            this.SettingCommand = new DelegateCommand(setting);
        }
        #endregion


        #region Function
        private async void setting()
        {
            await this.navigationService.NavigateAsync("Setting");
        }
        #endregion
    }
}
