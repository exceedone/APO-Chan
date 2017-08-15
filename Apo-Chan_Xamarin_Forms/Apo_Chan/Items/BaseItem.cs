using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace Apo_Chan.Items
{
    public abstract class BaseItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

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
