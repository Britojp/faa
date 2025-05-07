using Hl7.Fhir.Model;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IFhirParser
    {
        T Parse<T>(string json) where T : Resource;
    }
}