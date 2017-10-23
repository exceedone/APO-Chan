using System;
using Microsoft.Azure.Mobile.Server;
using Apo_ChanService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Apo_ChanService.DataObjects
{
    /// <summary>
    /// APO-Chan Group
    /// </summary>
    public class GroupUserItem : EntityData
    {
        public GroupUserItem()
            : base()
        {
            this.RefGroupId = null;
            this.RefUserId = null;
            this.AdminFlg = false;
            this.DeletedAt = null;
        }

        [Required]
        public string RefGroupId { get; set; }

        [ForeignKey("RefGroupId")]
        //[JsonIgnore]
        public GroupItem RefGroup { get; set; }

        [Required]
        public string RefUserId { get; set; }

        [ForeignKey("RefUserId")]
        //[JsonIgnore]
        public UserItem RefUser { get; set; }

        [Required]
        public bool AdminFlg { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? DeletedAt { get; set; }
    }
}