using Hl7.Fhir.Model;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{

    public interface IFhirValidatorService
    {
        OperationOutcome Validate(string fileName);
    }
}