using System.Text.Json.Serialization;

namespace FhirArtifactAnalyzer.Models
{
    /// <summary>
    /// Represents a link (edge) in the graph visualization
    /// </summary>
    public class GraphLink
    {
        /// <summary>
        /// ID of the source node
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// ID of the target node
        /// </summary>
        [JsonPropertyName("target")]
        public string Target { get; set; }

        /// <summary>
        /// Type of the relationship
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Description of the relationship
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The FHIR element path where the reference was found
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }

        /// <summary>
        /// Creates a GraphLink from source and target resources and a reference
        /// </summary>
        public static GraphLink FromResourceReference(string sourceUrl, string targetUrl, ResourceReference reference)
        {
            return new GraphLink
            {
                Source = sourceUrl,
                Target = targetUrl,
                Type = reference.Type,
                Path = reference.Path,
                Description = $"{reference.Type} reference from {sourceUrl} to {targetUrl}"
            };
        }
    }
}
