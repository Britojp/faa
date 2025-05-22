using System.Text.Json.Serialization;

namespace FhirArtifactAnalyzer.Models
{
    /// <summary>
    /// Represents a node in the graph visualization
    /// </summary>
    public class GraphNode
    {
        /// <summary>
        /// Unique identifier for the node
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Display label for the node
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Type of the FHIR resource
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Optional description for tooltips
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// X position in the graph visualization (set by D3.js)
        /// </summary>
        [JsonPropertyName("x")]
        public double? X { get; set; }

        /// <summary>
        /// Y position in the graph visualization (set by D3.js)
        /// </summary>
        [JsonPropertyName("y")]
        public double? Y { get; set; }

        /// <summary>
        /// The canonical URL of the resource
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Validation status for visual indicators
        /// </summary>
        [JsonPropertyName("validationStatus")]
        public string ValidationStatus { get; set; }

        /// <summary>
        /// Creates a GraphNode from a FhirResource
        /// </summary>
        public static GraphNode FromFhirResource(FhirResource resource)
        {
            return new GraphNode
            {
                Id = resource.CanonicalUrl ?? resource.Id,
                Label = !string.IsNullOrEmpty(resource.Name) ? resource.Name : resource.Id,
                Type = resource.ResourceType,
                Description = resource.Description,
                Url = resource.CanonicalUrl,
                ValidationStatus = resource.ValidationResults?.IsValid == true 
                    ? "valid" 
                    : resource.ValidationResults?.IsValid == false 
                        ? "invalid" 
                        : "unknown"
            };
        }
    }
}
