using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FhirArtifactAnalyzer.Models
{
    /// <summary>
    /// Represents the validation result for a FHIR artifact
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indicates if the resource is valid
        /// </summary>
        [JsonPropertyName("isValid")]
        public bool? IsValid { get; set; }

        /// <summary>
        /// List of validation issues
        /// </summary>
        [JsonPropertyName("issues")]
        public List<ValidationIssue> Issues { get; set; } = new List<ValidationIssue>();

        /// <summary>
        /// Indicates if the canonical URL is accessible
        /// </summary>
        [JsonPropertyName("canonicalUrlAccessible")]
        public bool? CanonicalUrlAccessible { get; set; }

        /// <summary>
        /// Get the severity of the most severe issue
        /// </summary>
        [JsonIgnore]
        public string HighestSeverity
        {
            get
            {
                if (Issues == null || Issues.Count == 0)
                    return "unknown";

                if (Issues.Exists(i => i.Severity == "error"))
                    return "error";
                    
                if (Issues.Exists(i => i.Severity == "warning"))
                    return "warning";
                    
                return "information";
            }
        }

        /// <summary>
        /// Get a summary message of the validation result
        /// </summary>
        public string GetSummary()
        {
            if (IsValid == true)
                return "Resource is valid";
                
            if (IsValid == false)
            {
                int errorCount = Issues.FindAll(i => i.Severity == "error").Count;
                int warningCount = Issues.FindAll(i => i.Severity == "warning").Count;
                
                return $"Found {errorCount} errors and {warningCount} warnings";
            }
            
            return "Validation not performed";
        }
    }

    /// <summary>
    /// Represents a single validation issue
    /// </summary>
    public class ValidationIssue
    {
        /// <summary>
        /// The severity of the issue (error, warning, information)
        /// </summary>
        [JsonPropertyName("severity")]
        public string Severity { get; set; }

        /// <summary>
        /// The issue code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// The element path where the issue was found
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// The issue message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
