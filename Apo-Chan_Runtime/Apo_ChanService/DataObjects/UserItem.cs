using System;
using Microsoft.Azure.Mobile.Server;
using Apo_ChanService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apo_ChanService.DataObjects
{
    public class UserItem : EntityData
    {
        public UserItem()
        {
            this.ProviderType = null;
            this.UserProviderId = null;
            this.UserName = null;
            this.Email = null;
        }
        
        //[Key]
        //public int UserId { get; set; }

        [Required]
        public Define.EProviderType? ProviderType { get; set; }

        [Index(IsUnique = true)]
        [Required]
        [MaxLength(128)]
        public string UserProviderId { get; set; }

        [MaxLength(128)]
        public string UserName { get; set; }

        [Index(IsUnique = true)]
        [Required]
        [MaxLength(128)]
        public string Email { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }
    }
}