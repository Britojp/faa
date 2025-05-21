using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace FhirArtifactAnalyzer.Models
{
    /// <summary>
    /// Represents statistics about analyzed FHIR artifacts
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// Total number of artifacts analyzed
        /// </summary>
        [JsonPropertyName("totalArtifacts")]
        public int TotalArtifacts { get; set; }

        /// <summary>
        /// Count of artifacts by resource type
        /// </summary>
        [JsonPropertyName("artifactsByType")]
        public Dictionary<string, int> ArtifactsByType { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Count of resources by validation status
        /// </summary>
        [JsonPropertyName("validationStatus")]
        public Dictionary<string, int> ValidationStatus { get; set; } = new Dictionary<string, int>
        {
            { "valid", 0 },
            { "invalid", 0 },
            { "unknown", 0 }
        };

        /// <summary>
        /// Number of references between resources
        /// </summary>
        [JsonPropertyName("totalReferences")]
        public int TotalReferences { get; set; }

        /// <summary>
        /// Generate statistics from a collection of FHIR resources
        /// </summary>
        public static Statistics GenerateFrom(IEnumerable<FhirResource> resources)
        {
            var stats = new Statistics();
            
            if (resources == null)
                return stats;
                
            var resourcesList = resources.ToList();
            stats.TotalArtifacts = resourcesList.Count;
            
            // Count artifacts by type
            stats.ArtifactsByType = resourcesList
                .GroupBy(r => r.ResourceType)
                .ToDictionary(g => g.Key, g => g.Count());
                
            // Count validation status
            foreach (var resource in resourcesList)
            {
                if (resource.ValidationResults == null)
                {
                    stats.ValidationStatus["unknown"]++;
                }
                else if (resource.ValidationResults.IsValid == true)
                {
                    stats.ValidationStatus["valid"]++;
                }
                else
                {
                    stats.ValidationStatus["invalid"]++;
                }
            }
            
            // Count total references
            stats.TotalReferences = resourcesList.Sum(r => r.References?.Count ?? 0);
            
            return stats;
        }
    }
}
