using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class GroupListViewModel : BaseViewModel, INavigatedAware
    {
        #region Variable and Property
        private ObservableCollection<GroupItem> groupItems;
        public ObservableCollection<GroupItem> GroupItems
        {
            get
            {
                return groupItems;
            }
            set
            {
                SetProperty(ref this.groupItems, value);
            }
        }
        
        private bool isCalledFromSetting;
        public bool IsCalledFromSetting 
        {
            get
            {
                return isCalledFromSetting;
            }
            set
            {
                SetProperty(ref this.isCalledFromSetting, value);
            }
        }

        /// <summary>
        /// Is called from report list
        /// </summary>
        public bool IsCalledFromReport
        {
            get
            {
                return !this.IsCalledFromSetting;
            }
            set
            {
                this.IsCalledFromSetting = !value;
            }
        }


        public DelegateCommand RefreshCommand { get; private set; }

        public DelegateCommand AddNewGroupCommand { get; private set; }

        public DelegateCommand<GroupItem> ItemTappedCommand { get; private set; }

        #endregion

        #region Constructor
        //constructor
        public GroupListViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            GroupItems = new ObservableCollection<GroupItem>();
            RefreshCommand = new DelegateCommand(SetItemsAsync).ObservesProperty(() => IsBusy);
            AddNewGroupCommand = new DelegateCommand(NavigateNewGroup);
            ItemTappedCommand = new DelegateCommand<GroupItem>(NavigateDetailGroup);

        }
        #endregion

        #region Function
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            ;
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            this.IsCalledFromReport = parameters.ContainsKey("IsCalledFromReport");
        }

        public async void SetItemsAsync()
        {
            if (!GlobalAttributes.isConnectedInternet)
            {
                await dialogService.DisplayAlertAsync("Error", "APO-Chan cannot connect to the Internet!", "OK");
                return;
            }
            await setItemsAsync();
        }

        public async void NavigateNewGroup()
        {
            await navigationService.NavigateAsync("NewGroup");
        }

        public async void NavigateDetailGroup(GroupItem item)
        {
            // If Called From Report, go back and add query groupid.
            if (this.IsCalledFromReport)
            {
                await navigationService.GoBackAsync(new NavigationParameters($"GroupId={item.Id}"));
            }
            else
            {
                await navigationService.NavigateAsync("DetailGroup?Id=" + item.Id);
            }
        }

        private async Task setItemsAsync()
        {
            IsBusy = true;

            ObservableCollection<GroupItem> allGroups = new ObservableCollection<GroupItem>();
            try
            {
                GroupItems.Clear();
                // Get Group List contains usercount and auth
                var groupCountList = await CustomFunction.Get<List<GroupAndUserCountItem>>($"api/values/userjoingroups/{GlobalAttributes.refUserId}");
                if (groupCountList != null)
                {
                    foreach (var g in groupCountList)
                    {
                        var group = g.Group;
                        group.UserCount = g.UserCount;
                        group.IsUserAdmin = g.AdminFlg;
                        // Add group image
                        await Service.ImageService.SetImageSource(group);
                        allGroups.Add(group);
                    }
                }
                this.GroupItems = allGroups;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
            }
            IsBusy = false;
        }

        #endregion

    }
}
