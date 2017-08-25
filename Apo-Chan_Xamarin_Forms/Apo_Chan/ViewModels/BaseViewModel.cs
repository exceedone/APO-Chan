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

        public DelegateCommand GoBackCommand { get; set; }

        public BaseViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            GoBackCommand = new DelegateCommand(ExecuteGoBack);
        }

        private async void ExecuteGoBack()
        {
            await navigationService.GoBackAsync();
        }
    }
}
