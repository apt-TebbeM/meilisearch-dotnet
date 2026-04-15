using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Federation options for queries on a specific index in federated multi-index search
    /// </summary>
    public class MultiSearchQueryFederationOptions
    {
        /// <summary>
        /// Use the weight property of the federation parameter to boost results coming from a specific query
        /// </summary>
        [JsonPropertyName("weight")]
        public decimal Weight { get; set; } = 1.0M;
    }
}
