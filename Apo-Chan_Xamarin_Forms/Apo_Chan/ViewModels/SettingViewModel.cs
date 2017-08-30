using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        #region Variable and Property
        public UserItem User { get; set; }

        private ObservableCollection<SettingMenuVMItem> settingItems;
        public ObservableCollection<SettingMenuVMItem> SettingItems
        {
            get
            {
                return settingItems;
            }
            set
            {
                SetProperty(ref this.settingItems, value);
            }
        }

        #endregion

        #region Constructor
        //constructor
        public SettingViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            this.User = UserItem.GetCachedUserItem();

            SettingItems = new ObservableCollection<SettingMenuVMItem>();
            SettingItems.Add(new SettingMenuVMItem("SignOut", "SignOut From APO-Chan.", navigateSignOut));
        }
        #endregion

        #region Function
        private async void navigateSignOut()
        {
            this.IsBusy = true;
            if (App.Authenticator != null)
            {
                await this.navigationService.NavigateAsync("/NavigationPage/SignIn");
            }
            //// Set syncItems to true to synchronize the data on startup when offline is enabled.
            //if (authenticated == true)
            //{
            //    await navigationService.NavigateAsync("SignIn");
            //}
            this.IsBusy = false;
        }

        #endregion

        #region InnerClass
        public class SettingMenuVMItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DelegateCommand ItemTappedCommand { get; private set; }

            public SettingMenuVMItem(string title, string description, Action executeMethod)
            {
                this.Title = title;
                this.Description = description;
                this.ItemTappedCommand = new DelegateCommand(executeMethod);
            }


        }
        #endregion
    }
}
