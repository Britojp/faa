using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IExtractorService
    {
        string Extract(string path, string destinationDirectory);
    }
}
