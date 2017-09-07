using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Apo_Chan.Items
{
    [DataContract(Name ="report")]
    public class ReportItem : BaseItem
    {
        private string refUserId;
        private string reportTitle;
        private string reportComment;
        private DateTime reportStartDate;
        private TimeSpan reportStartTime;
        private DateTime reportEndDate;
        private TimeSpan reportEndTime;
        private double reportLat;
        private double reportLon;

        [JsonProperty(PropertyName = "refUserId")]
        public string RefUserId
        {
            get
            {
                return refUserId;
            }
            set
            {
                SetProperty(ref this.refUserId, value);
            }
        }

        [JsonProperty(PropertyName = "reportTitle")]
        public string ReportTitle
        {
            get
            {
                return reportTitle;
            }
            set
            {
                SetProperty(ref this.reportTitle, value);
            }
        }

        [JsonProperty(PropertyName = "reportComment")]
        public string ReportComment
        {
            get
            {
                return reportComment;
            }
            set
            {
                SetProperty(ref this.reportComment, value);
            }
        }

        [JsonProperty(PropertyName = "reportStartDate")]
        public DateTime ReportStartDate
        {
            get
            {
                return reportStartDate;
            }
            set
            {
                SetProperty(ref this.reportStartDate, value);
            }
        }

        [JsonProperty(PropertyName = "reportStartTime")]
        public TimeSpan ReportStartTime
        {
            get
            {
                return reportStartTime;
            }
            set
            {
                SetProperty(ref this.reportStartTime, value);
            }
        }

        [JsonProperty(PropertyName = "reportEndDate")]
        public DateTime ReportEndDate
        {
            get
            {
                return reportEndDate;
            }
            set
            {
                SetProperty(ref this.reportEndDate, value);
            }
        }

        [JsonProperty(PropertyName = "reportEndTime")]
        public TimeSpan ReportEndTime
        {
            get
            {
                return reportEndTime;
            }
            set
            {
                SetProperty(ref this.reportEndTime, value);
            }
        }

        [JsonProperty(PropertyName = "reportLat")]
        public double ReportLat
        {
            get
            {
                return reportLat;
            }
            set
            {
                SetProperty(ref this.reportLat, value);
            }
        }

        [JsonProperty(PropertyName = "reportLon")]
        public double ReportLon
        {
            get
            {
                return reportLon;
            }
            set
            {
                SetProperty(ref this.reportLon, value);
            }
        }
    }
}
