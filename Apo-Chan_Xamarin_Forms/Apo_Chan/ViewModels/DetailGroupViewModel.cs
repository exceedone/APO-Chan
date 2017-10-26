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
    public class DetailGroupViewModel : BaseViewModel, INavigatedAware
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

        private GroupUserItem newGroupUser;
        public GroupUserItem NewGroupUser
        {
            get
            {
                return newGroupUser;
            }
            set
            {
                SetProperty(ref this.newGroupUser, value);
            }
        }

        private ObservableCollection<GroupUserItem> groupUserItems;
        public ObservableCollection<GroupUserItem> GroupUserItems
        {
            get
            {
                return groupUserItems;
            }
            set
            {
                SetProperty(ref this.groupUserItems, value);
            }
        }

        public DelegateCommand ImageSelectCommand { get; private set; }
        public DelegateCommand SubmitCommand { get; private set; }
        public DelegateCommand AddUserCommand { get; private set; }
        public DelegateCommand DeleteGroupCommand { get; private set; }
        public DelegateCommand<GroupUserItem> DeleteCommand { get; private set; }
        public IEnumerable<string> AuthPicker { get; }

        #endregion

        #region Constructor
        public DetailGroupViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            Group = new GroupItem();
            GroupUserItems = new ObservableCollection<GroupUserItem> ();
            NewGroupUser = new GroupUserItem();

            ImageSelectCommand = new DelegateCommand(imageSelect);
            SubmitCommand = new DelegateCommand(submitGroup);
            AddUserCommand = new DelegateCommand(addUser);
            DeleteGroupCommand = new DelegateCommand(deleteGroup);
            DeleteCommand = new DelegateCommand<GroupUserItem>(deleteUser);
            AuthPicker = Constants.AuthPicker.Select(x => x.Label).ToList();
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

                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
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

        private async void imageSelect()
        {
            if (this.Group.IsUserNotAdmin)
            {
                return;
            }
            this.IsBusy = true;
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsTakePhotoSupported)
            {
                await dialogService.DisplayAlertAsync("No Camera", "No camera available.", "OK");
                this.IsBusy = false;
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 500
            });
            if (file == null)
            {
                this.IsBusy = false;
                return;
            }
            this.Group.GroupImage = CustomImageSource.FromByteArray(() =>
            {
                var stream = file.GetStream();
                var byteArray = Utils.ReadStram(stream);
                file.Dispose();
                return byteArray;
            });
            this.IsBusy = false;
        }
        private async void submitGroup()
        {

            string message = this.getErrorMessageGroup();
            if (message != null)
            {
                await dialogService.DisplayAlertAsync
                (
                    "Error",
                    message,
                    "Confirm"
                );
                return;
            }

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
                    foreach (var item in this.GroupUserItems)
                    {
                        item.RefGroupId = Group.Id;

                        // Remove RefClass.
                        item.RefUser = null;
                        item.RefGroup = null;
                        await GroupUserManager.DefaultManager.SaveTaskAsync(item);
                    }

                    // upload icon
                    if (this.Group.HasImage)
                    {
                        await Service.ImageService.SaveImage(this.Group, this.Group.GroupImage.StreamByte);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
                }
                IsBusy = false;
                await navigationService.GoBackAsync();
            }
        }

        private async void addUser()
        {
            UserItem user = null;
            string message = this.getErrorMessageGroupUser();
            if (message == null)
            {
                user = await getUserByEmail();
                message = "The Email address you entered does not exist in accounts";
            }
            if (user != null)
            {
                var groupUser = new GroupUserItem()
                {
                    RefUser = user
                    ,
                    RefUserId = user.Id
                };
                this.GroupUserItems.Add(groupUser);
                // read user image
                await Service.ImageService.SetImageSource(groupUser.RefUser);
                this.NewGroupUser = new GroupUserItem();
            }
            else
            {
                await dialogService.DisplayAlertAsync
                     (
                         "Error",
                         message,
                         "OK"
                     );
                this.NewGroupUser.RefUser.Email = null;
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
                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
                }
                await navigationService.GoBackAsync();
                IsBusy = false;
            }
        }

        private async void deleteUser(GroupUserItem item)
        {
            string message = this.getErrorMessageDeleteGroupUser(item);
            if (message != null)
            {
                await dialogService.DisplayAlertAsync
                (
                    "Error",
                    message,
                    "Confirm"
                );
                return;
            }
            var accepted = await dialogService.DisplayAlertAsync
                (
                    "Confirmation",
                    $"Do you want to remove group user '{item.RefUser.NameAndEmail}'?",
                    "Confirm",
                    "Cancel"
                );
            if (accepted)
            {
                this.GroupUserItems.Remove(item);
            }
        }

        private string getErrorMessageGroup()
        {
            if (string.IsNullOrWhiteSpace(Group.GroupName))
            {
                return "Please input group name";
            }

            // Check Group User auth.
            foreach (var item in this.GroupUserItems)
            {
                // user auth is "user"
                if (item.RefUserId == GlobalAttributes.refUserId && !item.AdminFlg)
                {
                    return "Cannot set your admin 'User'";
                }
            }

            return null;
        }

        /// <summary>
        /// valid and get error message before search user from email.
        /// </summary>
        /// <returns></returns>
        private string getErrorMessageGroupUser()
        {
            string email = this.NewGroupUser.RefUser.Email;
            // empty textbox
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Please input email address.";
            }

            // same user email address
            if (GlobalAttributes.User.Email == email)
            {
                return "Please enter an email address other than your email address.";
            }

            foreach (var item in this.GroupUserItems)
            {
                if (item.RefUser.Email == email)
                {
                    return "The email address you entered is already included.";
                }
            }

            return null;
        }

        /// <summary>
        /// valid and get error message before delete user.
        /// </summary>
        /// <returns></returns>
        private string getErrorMessageDeleteGroupUser(GroupUserItem item)
        {
            // cannot delete userself.
            if (item.Id == GlobalAttributes.User.Id)
            {
                return "Cannot delete yourself.";
            }
            //cannot delete last 1.
            if (this.GroupUserItems.Count == 1)
            {
                return "Cannot delete last group user.";
            }
            //cannot delete last admin user.
            if (item.AdminFlg && this.GroupUserItems.Where(x => x.AdminFlg).Count() == 1)
            {
                return "Cannot delete last admin user.";
            }

            return null;
        }

        /// <summary>
        /// Check User From Email
        /// </summary>
        /// <returns></returns>
        private async Task<UserItem> getUserByEmail()
        {
            return await UsersManager.DefaultManager.GetItemAsync(x => x.Email == this.NewGroupUser.RefUser.Email);
        }

        #endregion
    }
}
