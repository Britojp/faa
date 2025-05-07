using FhirArtifactAnalyzer.Domain.Abstractions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirArtifactAnalyzer.Application.Services
{
    public class FhirParseService : IFhirParser
    {
        private readonly FhirJsonParser _jsonParser = new();

        public T Parse<T>(String resource) where T : Resource
        {
            return _jsonParser.Parse<T>(resource);
        }
    }
}