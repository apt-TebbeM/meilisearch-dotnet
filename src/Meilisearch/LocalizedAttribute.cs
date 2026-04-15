using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Object that holds localized attributes
    /// </summary>
    public class LocalizedAttribute
    {
        /// <summary>
        ///  ISO-639-3 three-letter locales or the  ISO-639-1 two-letter equivalents
        /// </summary>
        public List<string> Locales { get; set; } = new List<string>();

        /// <summary>
        /// Attribute patterns may begin or end with a * wildcard to match multiple fields: en_*, *-ar
        /// You may also set attributePatterns to *, in which case Meilisearch will treat all fields as if they were in the associated locale.
        /// </summary>
        public List<string> AttributePatterns { get; set; } = new List<string>();
    }
}
