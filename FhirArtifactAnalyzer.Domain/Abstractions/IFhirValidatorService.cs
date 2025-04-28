using Hl7.Fhir.Model;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{

    public interface IFhirValidatorService<T>
    {
        OperationOutcome Validate(T input);
    }
}