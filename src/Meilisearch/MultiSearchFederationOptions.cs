using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Federation options in federated multi-index search
    /// </summary>
    public class MultiSearchFederationOptions
    {
        /// <summary>
        /// Number of documents to skip
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Maximum number of documents returned
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Page for exhaustive paging (wins over the limit and offset parameters)
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>
        /// HitsPerPage for exhaustive paging (wins over the limit and offset parameters)
        /// </summary>
        [JsonPropertyName("hitsPerPage")]
        public int HitsPerPage { get; set; }
    }
}
