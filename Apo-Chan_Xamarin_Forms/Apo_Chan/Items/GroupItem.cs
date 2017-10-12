using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;
using Xamarin.Auth;
using Xamarin.Forms;
using Apo_Chan.Models;
using Apo_Chan.Managers;
using Newtonsoft.Json.Linq;

namespace Apo_Chan.Items
{
    [DataContract(Name = "group")]
    public class GroupItem : BaseItem
    {

        private string groupKey;
        private string groupName;
        private string createdUserId;
        private int userCount;
        private string userAuth;

        [JsonProperty(PropertyName = "groupKey")]
        public string GroupKey { get
            {
                return this.groupKey;
            }
            set
            {
                SetProperty(ref this.groupKey, value);
            }
        }

        [JsonProperty(PropertyName = "groupName")]
        public string GroupName
        {
            get
            {
                return this.groupName;
            }
            set
            {
                SetProperty(ref this.groupName, value);
            }
        }

        [JsonProperty(PropertyName = "createdUserId")]
        public string CreatedUserId
        {
            get
            {
                return this.createdUserId;
            }
            set
            {
                SetProperty(ref this.createdUserId, value);
            }
        }

        public int UserCount
        {
            get
            {
                return this.userCount;
            }
            set
            {
                SetProperty(ref this.userCount, value);
            }
        }

        public string UserAuth
        {
            get
            {
                return this.userAuth;
            }
            set
            {
                SetProperty(ref this.userAuth, value);
            }
        }


    }

}
