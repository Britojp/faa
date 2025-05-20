using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IFhirResourceSearcherStrategy
    {
        IEnumerable<FhirResource> Search(FhirResourceSearchParameters parameters);
    }
}
