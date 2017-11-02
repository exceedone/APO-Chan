using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
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
    [DataContract(Name = "reportgroup")]
    public class ReportGroupItem : BaseItem
    {
        private string refReportId;
        private string refGroupId;

        [JsonProperty(PropertyName = "refReportId")]
        public string RefReportId
        {
            get
            {
                return refReportId;
            }
            set
            {
                SetProperty(ref this.refReportId, value);
            }
        }

        [JsonProperty(PropertyName = "refGroupId")]
        public string RefGroupId
        {
            get
            {
                return refGroupId;
            }
            set
            {
                SetProperty(ref this.refGroupId, value);
            }
        }
    }
}
