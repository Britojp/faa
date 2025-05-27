using FhirArtifactAnalyzer.Domain.Constants;
using System.Text.RegularExpressions;

namespace FhirArtifactAnalyzer.Domain.Models
{
    public class FhirArtifactInfo
    {
        public InputSource Source { get; set; } = new InputSource();
        public string FilePath { get; set; } = string.Empty;
        public bool IsJson { get; set; }
        public bool IsWellFormedJson { get; set; }
        public bool IsRelevantFhirResource { get; set; }
        public string? ResourceType { get; set; }
        public string? ReasonIgnored { get; set; }
    }
}