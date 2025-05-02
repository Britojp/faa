using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IArtifactReader
    {
        bool CanRead(string filePath);

        /// <summary>
        /// Lê o arquivo fornecido e retorna uma lista de objetos FHIR relevantes (StructureDefinition, CodeSystem, etc).
        /// </summary>
        IEnumerable<string> ReadArtifacts(string filePath);
    }
}
