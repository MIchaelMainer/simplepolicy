using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace simplepolicy
{
    internal class ContextPayload
    {

        [JsonPropertyName("identity_context")]
        public IdentityContext IdentityContext { get; set; }

        [JsonPropertyName("policy_context")]
        public PolicyContext PolicyContext { get; set; }
    }

    internal class IdentityContext
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("identity")]
        public string Identity { get; set; }
    }

    internal class PolicyContext
    {

        [JsonPropertyName("decisions")]
        public List<string> Decisions { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
    }

    internal class DecisionResult
    {
        [JsonPropertyName("decisions")]
        public List<Decision> Decisions { get; set; }
    }

    internal class Decision
    {
        [JsonPropertyName("decision")]
        public string DecisionType { get; set; } // ugh

        [JsonPropertyName("is")]
        public bool Is { get; set; }

    }


}
