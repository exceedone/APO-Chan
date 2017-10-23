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
using Apo_Chan.Models;

namespace Apo_Chan.Items
{
    [DataContract(Name = "groupuser")]
    public class GroupUserItem : BaseItem
    {
        public GroupUserItem()
        {
            this.RefUser = new UserItem();
            this.RefGroup = new GroupItem();
        }

        private string refGroupId;
        private string refUserId;
        private bool adminFlg;
        private int? authSelectedIndex;

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

        [JsonProperty]
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

        [JsonProperty]
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

        /// <summary>
        /// Selectrd Auth
        /// </summary>
        public string Auth
        {
            get
            {
                return Constants.AuthPicker.FirstOrDefault(x => x.AdminFlg == this.AdminFlg)?.Label ?? string.Empty;
            }
            set
            {
                this.AdminFlg = Constants.AuthPicker.FirstOrDefault(x => x.Label == value)?.AdminFlg ?? false;
            }
        }

        public int AuthSelectedIndex
        {
            get
            {
                if (!this.authSelectedIndex.HasValue)
                {
                    this.authSelectedIndex = Constants.AuthPicker.Select((auth, index) => new { auth, index }).FirstOrDefault(x => x.auth.AdminFlg == this.AdminFlg).index;
                }
                return authSelectedIndex.Value;
            }
            set
            {
                // sometimes set as -1, but it's error, so return.
                if(value == -1)
                {
                    return;
                }

                if (authSelectedIndex != value)
                {
                    // trigger some action to take such as updating other labels or fields
                    SetProperty(ref this.authSelectedIndex, value);
                    this.Auth = Constants.AuthPicker[authSelectedIndex ?? 0].Label;
                }
            }
        }

    }

}
