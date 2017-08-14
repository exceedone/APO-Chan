using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace Apo_Chan.Models
{
    public class Report
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "RefUserId")]
        public string RefUserId { get; set; }

        [JsonProperty(PropertyName = "ReportTitle")]
        public string ReportTitle { get; set; }

        [JsonProperty(PropertyName = "ReportComment")]
        public string ReportComment { get; set; }

        [JsonProperty(PropertyName = "ReportStartDate")]
        public DateTime ReportStartDate { get; set; }

        [JsonProperty(PropertyName = "ReportStartTime")]
        public TimeSpan ReportStartTime { get; set; }

        [JsonProperty(PropertyName = "ReportEndDate")]
        public DateTime ReportEndDate { get; set; }

        [JsonProperty(PropertyName = "ReportEndTime")]
        public TimeSpan ReportEndTime { get; set; }

        [JsonProperty(PropertyName = "ReportLat")]
        public double ReportLat { get; set; }

        [JsonProperty(PropertyName = "ReportLon")]
        public double ReportLon { get; set; }

        [Version]
        public string Version { get; set; }

        [CreatedAt]
        public DateTimeOffset CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset UpdatedAt { get; set; }

        [Deleted]
        public Boolean Deleted { get; set; }

    }
}
