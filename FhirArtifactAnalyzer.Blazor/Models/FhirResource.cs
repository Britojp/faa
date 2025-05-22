using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FhirArtifactAnalyzer.Models
{
    /// <summary>
    /// Represents a FHIR resource extracted from an artifact
    /// </summary>
    public class FhirResource
    {
        /// <summary>
        /// The resource identifier
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The canonical URL of the resource
        /// </summary>
        [JsonPropertyName("url")]
        public string CanonicalUrl { get; set; }

        /// <summary>
        /// The type of FHIR resource
        /// </summary>
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        /// <summary>
        /// The version of the resource
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// The name of the resource
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The title of the resource, if available
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The description of the resource
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The status of the resource (e.g., draft, active)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// The path or URL where the resource was found
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }

        /// <summary>
        /// The size of the resource file in bytes
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// List of references to other resources
        /// </summary>
        [JsonPropertyName("references")]
        public List<ResourceReference> References { get; set; } = new List<ResourceReference>();

        /// <summary>
        /// Validation results for this resource
        /// </summary>
        [JsonPropertyName("validationResults")]
        public ValidationResult ValidationResults { get; set; }

        /// <summary>
        /// Formatted display name for UI
        /// </summary>
        [JsonIgnore]
        public string DisplayName => string.IsNullOrEmpty(Title) ? Name : Title;

        /// <summary>
        /// Get a formatted string showing resource type and name
        /// </summary>
        public string GetFormattedName()
        {
            return $"{ResourceType}: {DisplayName}";
        }

        /// <summary>
        /// Check if the resource contains a specific search term
        /// </summary>
        public bool ContainsSearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return true;

            searchTerm = searchTerm.ToLowerInvariant();
            
            return 
                (Name?.ToLowerInvariant()?.Contains(searchTerm) ?? false) ||
                (Title?.ToLowerInvariant()?.Contains(searchTerm) ?? false) ||
                (Description?.ToLowerInvariant()?.Contains(searchTerm) ?? false) ||
                (CanonicalUrl?.ToLowerInvariant()?.Contains(searchTerm) ?? false) ||
                (ResourceType?.ToLowerInvariant()?.Contains(searchTerm) ?? false);
        }
    }

    /// <summary>
    /// Represents a reference to another resource
    /// </summary>
    public class ResourceReference
    {
        /// <summary>
        /// The reference type (e.g., canonical, uri, Reference)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The referenced resource URL or identifier
        /// </summary>
        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        /// <summary>
        /// The element path where the reference was found
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}
