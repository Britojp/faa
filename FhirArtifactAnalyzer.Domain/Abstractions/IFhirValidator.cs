using Hl7.Fhir.Model;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{

    public interface IFhirValidator
    {
        OperationOutcome Validate(string fileName);
    }
}