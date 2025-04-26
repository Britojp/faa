using Hl7.Fhir.Model;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{

    public interface IFhirParseService
    {
        Resource Parse(Stream inputStream, string format);
    }
}