using System.Collections.Generic;

namespace FhirArtifactAnalyzer.Models
{
    /// <summary>
    /// Represents filter criteria for FHIR artifacts
    /// </summary>
    public class ArtifactFilter
    {
        /// <summary>
        /// Resource types to include in the filter
        /// </summary>
        public List<string> ResourceTypes { get; set; } = new List<string>();

        /// <summary>
        /// Validation status to filter by
        /// </summary>
        public string ValidationStatus { get; set; }

        /// <summary>
        /// Text search across resource properties
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Filter by reference to a specific resource
        /// </summary>
        public string ReferencedResource { get; set; }

        /// <summary>
        /// Apply this filter to a collection of resources
        /// </summary>
        public List<FhirResource> Apply(IEnumerable<FhirResource> resources)
        {
            var result = new List<FhirResource>();
            
            foreach (var resource in resources)
            {
                // Filter by resource type
                if (ResourceTypes.Count > 0 && !ResourceTypes.Contains(resource.ResourceType))
                    continue;
                    
                // Filter by validation status
                if (!string.IsNullOrEmpty(ValidationStatus))
                {
                    if (ValidationStatus == "valid" && resource.ValidationResults?.IsValid != true)
                        continue;
                    if (ValidationStatus == "invalid" && resource.ValidationResults?.IsValid != false)
                        continue;
                }
                
                // Filter by search text
                if (!string.IsNullOrEmpty(SearchText) && !resource.ContainsSearchTerm(SearchText))
                    continue;
                    
                // Filter by referenced resource
                if (!string.IsNullOrEmpty(ReferencedResource))
                {
                    bool hasReference = false;
                    foreach (var reference in resource.References)
                    {
                        if (reference.Reference == ReferencedResource)
                        {
                            hasReference = true;
                            break;
                        }
                    }
                    
                    if (!hasReference)
                        continue;
                }
                
                result.Add(resource);
            }
            
            return result;
        }
    }
}
