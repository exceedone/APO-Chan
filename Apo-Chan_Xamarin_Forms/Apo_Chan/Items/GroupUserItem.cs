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
    [DataContract(Name = "groupuser")]
    public class GroupUserItem : BaseItem
    {
        private string refGroupId;
        private string refUserId;
        private bool adminFlg;

        [JsonProperty(PropertyName = "refGroupId")]
        public string RefGroupId
        {
            get
            {
                return this.refGroupId;
            }
            set
            {
                SetProperty(ref this.refGroupId, value);
            }
        }

        public GroupItem RefGroup { get; set; }

        [JsonProperty(PropertyName = "refUserId")]
        public string RefUserId
        {
            get
            {
                return this.refUserId;
            }
            set
            {
                SetProperty(ref this.refUserId, value);
            }
        }

        public UserItem RefUser { get; set; }

        [JsonProperty(PropertyName = "adminFlg")]
        public bool AdminFlg
        {
            get
            {
                return this.adminFlg;
            }
            set
            {
                SetProperty(ref this.adminFlg, value);
            }
        }

    }

}
