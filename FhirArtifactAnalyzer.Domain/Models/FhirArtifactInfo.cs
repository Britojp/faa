using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirArtifactAnalyzer.Domain.Models
{
    public class FhirArtifactInfo
    {
        public string FilePath { get; set; }
        public bool IsJson { get; set; }
        public bool IsWellFormedJson { get; set; }
        public bool IsRelevantFhirResource { get; set; }
        public string? ResourceType { get; set; } 
        public string? ReasonIgnored { get; set; }
    }
}
