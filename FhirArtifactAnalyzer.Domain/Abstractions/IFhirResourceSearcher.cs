using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IFhirResourceSearcher
    {
        /// <summary>
        /// Searches for FHIR resources based on the provided search parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<FhirResource> Search(
            FhirResourceSearchParameters parameters,
            SearchQueryOperator @operator = SearchQueryOperator.Or,
            bool enableSubstringSearch = false);

        /// <summary>
        /// Searches for FHIR resources based on the provided search term.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<FhirResource> Search(string searchTerm, bool enableSubstringSearch = false);
    }
}
