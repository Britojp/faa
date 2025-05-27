using FhirArtifactAnalyzer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IArtifactProcessor
    {
        Task<IEnumerable<FhirArtifactInfo>> ProcessInputAsync(InputSource source);
    }
}
