using Apo_Chan.Geolocation;
using Apo_Chan.Items;
using Apo_Chan.Managers;
using Apo_Chan.Models;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class DetailGroupViewModel : BaseGroupViewModel, INavigatedAware
    {
        #region Variable and Property
        
        public DelegateCommand DeleteGroupCommand { get; private set; }
        #endregion

        #region Constructor
        public DetailGroupViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            DeleteGroupCommand = new DelegateCommand(deleteGroup);
        }
        #endregion

        #region Function

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            ;
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            this.GroupUserItems.Clear();
            var allGroupUserItems = new ObservableCollection<GroupUserItem>();
            if (parameters.ContainsKey("Id"))
            {
                IsBusy = true;
                try
                {
                    // get group and groupusers
                    var item = await CustomFunction.Get<GroupAndGroupUsersItem>($"api/values/groupusers/{(string)parameters["Id"]}");
                    if (item != null)
                    {
                        foreach (var user in item.GroupUsers)
                        {
                            if(user != null)
                            {
                                allGroupUserItems.Add(user);
                                // if userself item, set adminflg
                                if(user.RefUserId == GlobalAttributes.refUserId)
                                {
                                    item.Group.IsUserAdmin = user.AdminFlg;
                                }
                                // Add group user image
                                await Service.ImageService.SetImageSource(user.RefUser);
                            }
                        }
                        this.Group = item.Group;
                        await Service.ImageService.SetImageSource(this.Group);
                    }
                }
                catch (Exception e)
                {

                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] DetailGroupViewModel > " + e.Message);
                }
                this.GroupUserItems = allGroupUserItems;
                IsBusy = false;
            }
            else
            {
                await dialogService.DisplayAlertAsync("Error", "Failed to load the detail page!", "OK");
                await navigationService.GoBackAsync();
            }
        }

        
        
        private async void deleteGroup()
        {
            var accepted = await dialogService.DisplayAlertAsync
                (
                    "Confirmation",
                    "Do you want to delete the report?",
                    "Confirm",
                    "Cancel"
                );
            if (accepted)
            {
                if (!GlobalAttributes.isConnectedInternet)
                {
                    await dialogService.DisplayAlertAsync("Error", "APO-Chan cannot connect to the Internet!", "OK");
                    return;
                }
                IsBusy = true;
                try
                {
                    await GroupManager.DefaultManager.DeleteAsync(Group);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] DetailGroupViewModel > " + e.Message);
                }
                await navigationService.GoBackAsync();
                IsBusy = false;
            }
        }

        protected override void imageSelect()
        {
            if (this.Group.IsUserNotAdmin)
            {
                return;
            }
            base.imageSelect();
        }

        #endregion
    }
}
