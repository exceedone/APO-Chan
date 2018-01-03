using System;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;

namespace Apo_Chan.Items
{
    public abstract class BaseItem: BindableBase
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Version]
        public byte[] Version { get; set; }

        [CreatedAt]
        public DateTimeOffset CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "deleted")]
        [Required]
        [Deleted]
        public bool Deleted { get; set; }

        [JsonProperty(PropertyName = "deletedAt")]
        public DateTimeOffset? DeletedAt { get; set; }

    }
}
