using Apo_ChanService.Models;
using Microsoft.Azure.Mobile.Server;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apo_ChanService.DataObjects
{
    public class ReportItem : EntityData
    {
        public ReportItem()
            : base()
        {
            this.ReportTitle = null;
            this.ReportComment = null;
            this.ReportStartDate = null;
            this.ReportStartTime = null;
            this.ReportEndDate = null;
            this.ReportEndTime = null;
        }

        [Required]
        [ForeignKey("RefUser")]
        public string RefUserId { get; set; }

        [MaxLength(256)]
        public string ReportTitle { get; set; }

        [MaxLength(4000)]
        public string ReportComment { get; set; }

        [Column(TypeName = "Date")]
        //[JsonConverter(typeof(CustomAttributes.DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? ReportStartDate { get; set; }

        [Column(TypeName = "Time")]
        public TimeSpan? ReportStartTime { get; set; }

        [Column(TypeName = "Date")]
        //[JsonConverter(typeof(CustomAttributes.DateFormatConverter))]//YYYYMMDD必要？
        public DateTime? ReportEndDate { get; set; }

        [Column(TypeName = "Time")]
        public TimeSpan? ReportEndTime { get; set; }

        [CustomAttributes.DecimalPrecision(10, 7)]
        public decimal? ReportLat { get; set; }

        [CustomAttributes.DecimalPrecision(10, 7)]
        public decimal? ReportLon { get; set; }

        [MaxLength(512)]
        public string ReportAddress { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        //[JsonIgnore]
        public UserItem RefUser { get; set; }

    }
}