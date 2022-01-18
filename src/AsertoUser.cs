using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace simplepolicy
{
    internal class UserPayload
    {
        [JsonPropertyName("results")]
        public List<AsertoUser> Results { get; set; }
    }

    internal class AsertoUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }
}
