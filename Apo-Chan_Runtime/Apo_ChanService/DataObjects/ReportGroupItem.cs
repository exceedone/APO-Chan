using Apo_ChanService.Models;
using Microsoft.Azure.Mobile.Server;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apo_ChanService.DataObjects
{
    public class ReportGroupItem : EntityData
    {
        public ReportGroupItem()
            : base()
        {
        }

        [Required]
        [ForeignKey("RefReport")]
        public string RefReportId { get; set; }

        [Required]
        [ForeignKey("RefGroup")]
        public string RefGroupId { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        [JsonIgnore]
        public ReportItem RefReport { get; set; }

        [JsonIgnore]
        public GroupItem RefGroup { get; set; }

    }
}