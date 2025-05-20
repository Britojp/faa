using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IFhirResourceSearcher
    {
        IEnumerable<FhirResource> Search(
            FhirResourceSearchParameters parameters,
            bool enableSubstringSearch = false,
            SearchQueryOperator @operator = SearchQueryOperator.Or);

        IEnumerable<FhirResource> Search(string searchTerm, bool enableSubstringSearch = false);
    }
}
