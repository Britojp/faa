using FhirArtifactAnalyzer.Domain.Abstractions;
using Hl7.Fhir.Model;

namespace FhirArtifactAnalyzer.Application.Services
{
    public class ArtifactValidation
    {
        private readonly IFhirValidator _validator;

        public ArtifactValidation(IFhirValidator validator)
        {
            _validator = validator;
        }

        public OperationOutcome Execute(string fileName)
        {
            return _validator.Validate(fileName);
        }
    }
}