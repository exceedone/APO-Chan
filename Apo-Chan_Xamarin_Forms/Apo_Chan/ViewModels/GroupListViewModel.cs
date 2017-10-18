using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class GroupListViewModel : BaseViewModel
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
            await navigationService.NavigateAsync("DetailGroup?Id=" + item.Id);
        }

        private async Task setItemsAsync()
        {
            IsBusy = true;

            ObservableCollection<GroupItem> allGroups = null;
            try
            {
                allGroups = await GroupManager.DefaultManager.GetItemsAsync();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
            }

            GroupItems.Clear();
            if (allGroups != null)
            {
                foreach (var item in allGroups)
                {
                    // get all group's user count and auth.
                    // TODO: I think it's very slowly manner. 
                    // I want to get groupusers when we get groups at the same time. 
                    try
                    {
                        var groupItems = await GroupUserManager.DefaultManager.GetItemsAsync(x => x.RefGroupId == item.Id);
                        item.UserCount = groupItems.Count;
                        var isAdmin = groupItems.Where(x => x.RefUserId == GlobalAttributes.refUserId).FirstOrDefault()?.AdminFlg ?? false;
                        item.UserAuth = isAdmin ? "Admin" : "User";
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            if (allGroups != null)
            {
                foreach (var item in allGroups)
                {
                    await Service.ImageService.SetImageSource(item);
                    GroupItems.Add(item);
                }
            }
            IsBusy = false;
        }

        #endregion

    }
}
