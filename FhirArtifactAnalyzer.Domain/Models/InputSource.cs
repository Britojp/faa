using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirArtifactAnalyzer.Domain.Models
{
    public record InputSource
    {
        public string PathOrUrl { get; init; } = string.Empty;
        public InputType Type { get; init; }
        public string? RegexFilter { get; init; }
     }
}
