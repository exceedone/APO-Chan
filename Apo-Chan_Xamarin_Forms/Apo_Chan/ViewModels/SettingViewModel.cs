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
using Apo_Chan.Models;

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
            this.User = GlobalAttributes.User;
            //if (!string.IsNullOrEmpty(this.User.UserImageBase64))
            //{
            //    this.User.UserImage = Utils.ImageFromBase64(this.User.UserImageBase64);
            //}
            //else
            //{
            //    this.User.UserImage = new Image() { Source = "icon_account.png" };
            //}

            SettingItems = new ObservableCollection<SettingMenuVMItem>();
            SettingItems.Add(new SettingMenuVMItem("Manage Group", "Manage groups that share reports.", openManageGroup));
            SettingItems.Add(new SettingMenuVMItem("See the Source Code", "Access GitHub to see the source code of APO-Chan.", openGithub));
            SettingItems.Add(new SettingMenuVMItem("About us", "Access ExceedOne Homepage.", openExcedOne));
            SettingItems.Add(new SettingMenuVMItem("Version", Xamarin.Forms.DependencyService.Get<IAssemblyService>().GetVersionName(), versionStub));
            SettingItems.Add(new SettingMenuVMItem("SignOut", "SignOut From APO-Chan.", navigateSignOut));
        }
        #endregion

        #region Function
        private async void navigateSignOut()
        {
            this.IsBusy = true;
            bool isLogOut = false;
            if (App.Authenticator != null)
            {
                isLogOut = await App.Authenticator.SignOutAsync();
                if (isLogOut)
                {
                    UserItem.ClearUserToken();
                    await this.navigationService.NavigateAsync("/NavigationPage/SignIn");
                }
            }

            this.IsBusy = false;
        }

        /// <summary>
        /// Open browser
        /// </summary>
        private async void openManageGroup()
        {
            await this.navigationService.NavigateAsync("GroupList");
        }

        /// <summary>
        /// Open browser
        /// </summary>
        private void openGithub()
        {
            Device.OpenUri(new Uri(Constants.GitHubURL));
        }

        /// <summary>
        /// Open browser
        /// </summary>
        private void openExcedOne()
        {
            Device.OpenUri(new Uri(Constants.ExcedOneURL));
        }

        /// <summary>
        /// Stub method for version item
        /// </summary>
        private void versionStub()
        {
            ;
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
