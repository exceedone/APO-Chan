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
    public class NewGroupViewModel : BaseGroupViewModel
    {
        #region Variable and Property

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
            // Init and add yourself.
            var myself = new GroupUserItem()
            {
                RefGroupId = Group.Id
                            ,
                RefUser = GlobalAttributes.User
                            ,
                RefUserId = GlobalAttributes.refUserId
                            ,
                AdminFlg = true
            };
            Task.Run(() => Service.ImageService.SetImageSource(myself.RefUser));
            GroupUserItems = new ObservableCollection<GroupUserItem>{ myself };
            NewGroupUser = new GroupUserItem();

            ImageSelectCommand = new DelegateCommand(imageSelect);
            SubmitCommand = new DelegateCommand(submitGroup);
            AddUserCommand = new DelegateCommand(addUser);
            DeleteCommand = new DelegateCommand<GroupUserItem>(deleteUser);
            AuthPicker = Constants.AuthPicker.Select(x => x.Label).ToList();
        }
        #endregion

        #region Function
        private async void imageSelect()
        {
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
        
        #endregion
    }
}
