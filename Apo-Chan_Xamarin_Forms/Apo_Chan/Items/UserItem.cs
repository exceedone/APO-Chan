using System;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using Apo_Chan.Models;
using System.Runtime.Serialization;

namespace Apo_Chan.Items
{
    [DataContract(Name = "report")]
    public class UserItem: BaseItem
    {
        [JsonProperty(PropertyName = "ProviderType")]
        public int ProviderType { get; set; }

        [JsonProperty(PropertyName = "UserProviderId")]
        public string UserProviderId { get; set; }

        [JsonProperty(PropertyName = "UserName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "DeletedAt")]
        public DateTime DeletedAt { get; set; }
    }
}
