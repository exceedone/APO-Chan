using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using Prism.Mvvm;

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

        [Deleted]
        public bool Deleted { get; set; }

        [JsonProperty(PropertyName = "deletedAt")]
        public DateTimeOffset? DeletedAt { get; set; }

    }
}
