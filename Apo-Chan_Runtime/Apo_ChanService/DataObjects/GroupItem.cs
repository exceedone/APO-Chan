using System;
using Microsoft.Azure.Mobile.Server;
using Apo_ChanService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apo_ChanService.DataObjects
{
    /// <summary>
    /// APO-Chan Group
    /// </summary>
    public class GroupItem : EntityData
    {
        public GroupItem()
            : base()
        {
            this.GroupKey = null;
            this.GroupName = null;
            this.CreatedUserId = null;
            this.DeletedAt = null;
        }

        [Index(IsUnique = true)]
        [Required]
        [MaxLength(256)]
        public string GroupKey { get; set; }
        [Required]
        [MaxLength(256)]
        public string GroupName { get; set; }
        [Required]
        [MaxLength(128)]
        public string CreatedUserId { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? DeletedAt { get; set; }
    }
}