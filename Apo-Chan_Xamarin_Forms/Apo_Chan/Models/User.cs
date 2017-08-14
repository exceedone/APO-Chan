using System;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace Apo_Chan.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

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
