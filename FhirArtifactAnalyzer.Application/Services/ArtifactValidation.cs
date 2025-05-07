using FhirArtifactAnalyzer.Domain.Abstractions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath.Sprache;

namespace FhirArtifactAnalyzer.Application.Services
{

    public class ArtifactValidation 
    {
        private readonly IFhirParseService _parser;
        private readonly IFhirValidatorService<Resource> _validator;

        public ArtifactValidation(
            IFhirParseService parser,
            IFhirValidatorService<Resource> validator)
        {
            _parser = parser;
            _validator = validator;
        }

        public OperationOutcome Execute(Stream inputStream, string format)
        {
            var resource = _parser.Parse(inputStream, format);
            return _validator.Validate(resource);
        }
    }
}