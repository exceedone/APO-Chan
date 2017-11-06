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
        /// <summary>
        /// select group id list (for called report view)
        /// </summary>
        private List<string> selectGroupList;

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

        /// <summary>
        /// Type of screen called
        /// </summary>
        public int CalledType
        {
            set
            {
                this.IsCalledFromSetting = (value == 0);
                this.IsCalledFromReportList = (value == 1);
                this.IsCalledFromReport = (value == 2);
            }
        }

        private bool isCalledFromSetting;
        public bool IsCalledFromSetting
        {
            get
            {
                return isCalledFromSetting;
            }
            private set
            {
                SetProperty(ref this.isCalledFromSetting, value);
            }
        }

        private bool isCalledFromReportList;
        /// <summary>
        /// Is called from report list
        /// </summary>
        public bool IsCalledFromReportList
        {
            get
            {
                return isCalledFromReportList;
            }
            private set
            {
                SetProperty(ref this.isCalledFromReportList, value);
            }
        }

        private bool isCalledFromReport;
        /// <summary>
        /// Is called from report list
        /// </summary>
        public bool IsCalledFromReport
        {
            get
            {
                return isCalledFromReport;
            }
            private set
            {
                SetProperty(ref this.isCalledFromReport, value);
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
            selectGroupList = new List<string>();
            GroupItems = new ObservableCollection<GroupItem>();
            RefreshCommand = new DelegateCommand(SetItemsAsync).ObservesProperty(() => IsBusy);
            AddNewGroupCommand = new DelegateCommand(NavigateNewGroup);
            ItemTappedCommand = new DelegateCommand<GroupItem>(NavigateDetailGroup);
        }
        #endregion

        #region Function
        /// <summary>
        /// back from this page
        /// </summary>
        /// <param name="parameters"></param>
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            if (IsCalledFromReport)
            {

                var toggleList = this.GroupItems.Where(x => x.IsSelect);
                parameters.Add("GroupId", string.Join(",", toggleList.Select(x => x.Id)));
                parameters.Add("GroupName", string.Join(",", toggleList.Select(x => Flurl.Url.EncodeQueryParamValue(x.GroupName, false))));
            }
            else if (IsCalledFromReportList)
            {
                // if flow back, add Reset parameter.
                if (parameters.GetNavigationMode() == NavigationMode.Back)
                {
                    parameters.Add("Reset", true);
                }
            }
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("CalledType"))
            {
                this.CalledType = Convert.ToInt32(parameters["CalledType"]);
                // when CalledType = 2(From Report) and has GroupId, set selectGroupList
                if (this.IsCalledFromReport && parameters.ContainsKey("GroupId"))
                {
                    this.selectGroupList = parameters["GroupId"].ToString().Split(',').ToList();
                }
            }
            else
            {
                this.CalledType = 0;
            }

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
            // If Called From ReportList, go back and add query groupid.
            if (this.IsCalledFromReportList)
            {
                await this.navigateTop($"GroupId={item.Id}&GroupName={Flurl.Url.EncodeQueryParamValue(item.GroupName, false)}");
            }
            // If Called From Setting, go detailgroup.
            else if (this.IsCalledFromSetting)
            {
                await navigationService.NavigateAsync("DetailGroup?Id=" + item.Id);
            }
            // If called from Report, toggle.
            else
            {
                item.IsSelect = !item.IsSelect;
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
                        // g.Id has selectGroupList, group.IsSelect is true.
                        group.IsSelect = this.selectGroupList.Contains(g.Group.Id);
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
