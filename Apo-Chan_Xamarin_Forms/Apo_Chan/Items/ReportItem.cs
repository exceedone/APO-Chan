using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace Apo_Chan.Items
{
    [DataContract(Name ="report")]
    public class ReportItem : BaseItem
    {
        [JsonProperty(PropertyName = "refUserId")]
        public string RefUserId { get; set; }

        [JsonProperty(PropertyName = "reportTitle")]
        public string ReportTitle { get; set; }

        [JsonProperty(PropertyName = "reportComment")]
        public string ReportComment { get; set; }

        [JsonProperty(PropertyName = "reportStartDate")]
        public DateTime ReportStartDate { get; set; }

        [JsonProperty(PropertyName = "reportStartTime")]
        public TimeSpan ReportStartTime { get; set; }

        [JsonProperty(PropertyName = "reportEndDate")]
        public DateTime ReportEndDate { get; set; }

        [JsonProperty(PropertyName = "reportEndTime")]
        public TimeSpan ReportEndTime { get; set; }

        [JsonProperty(PropertyName = "reportLat")]
        public double ReportLat { get; set; }

        [JsonProperty(PropertyName = "reportLon")]
        public double ReportLon { get; set; }
    }
}
