using FhirArtifactAnalyzer.Domain.Abstractions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirArtifactAnalyzer.Application.Parsers
{
    public class JsonFhirParser : IFhirParser
    {
        private readonly FhirJsonParser _parser = new();

        public T Parse<T>(string resource) where T : Resource
        {
            return _parser.Parse<T>(resource);
        }
    }
}