using FhirArtifactAnalyzer.Domain.Abstractions;
using Hl7.Fhir.Model;

namespace FhirArtifactAnalyzer.Application.Services
{

    public class ArtifactValidation 
    {
        private readonly IFhirValidatorService _validator;

        public ArtifactValidation(IFhirValidatorService validator)
        {
            _validator = validator;
        }

        public OperationOutcome Execute(string fileName)
        {
            return _validator.Validate(fileName);
        }
    }
}