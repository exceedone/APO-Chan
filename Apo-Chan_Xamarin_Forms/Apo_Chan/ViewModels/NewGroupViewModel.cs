using Apo_Chan.Geolocation;
using Apo_Chan.Items;
using Apo_Chan.Managers;
using Apo_Chan.Models;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class NewGroupViewModel : BaseViewModel
    {
        #region Variable and Property
        private GroupItem group;
        public GroupItem Group
        {
            get
            {
                return group;
            }
            set
            {
                SetProperty(ref this.group, value);
            }
        }
        public DelegateCommand SubmitCommand { get; private set; }
        #endregion

        #region Constructor
        public NewGroupViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            Group = new GroupItem
            {
                Id = null,
                CreatedUserId = GlobalAttributes.refUserId,
                GroupName = null,
            };

            SubmitCommand = new DelegateCommand(submitGroup);
        }
#endregion

#region Function
        private async void submitGroup()
        {
            if (isValidGroup())
            {
                var accepted = await dialogService.DisplayAlertAsync
                    (
                        "Confirmation",
                        "Do you want to submit this group?",
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
                        // Save group
                        await GroupManager.DefaultManager.SaveTaskAsync(Group);

                        // Create groupuser and save.
                        await GroupUserManager.DefaultManager.SaveTaskAsync(new GroupUserItem()
                        {
                            RefGroupId = Group.Id
                            , RefUserId = GlobalAttributes.refUserId
                            , AdminFlg = true
                        });
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
                    }
                    IsBusy = false;
                    await navigationService.GoBackAsync();
                }
            }
        }

        private bool isValidGroup()
        {
            bool isValid = false;
            isValid = !string.IsNullOrWhiteSpace(Group.GroupName);

            return isValid;
        }

        #endregion
    }
}
