using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Application.Services
{
    public class FhirResourceSearcher(IFhirResourceSearcherStrategy searcher)
    {
        /// <summary>
        /// Searches for FHIR resources based on the provided search parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<FhirResource> Search(FhirResourceSearchParameters parameters)
        {
            return searcher.Search(parameters);
        }

        /// <summary>
        /// Searches for FHIR resources based on the provided search term.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public IEnumerable<FhirResource> Search(string searchTerm)
        {
            return searcher.Search(new FhirResourceSearchParameters(
                Id: null,
                Name: searchTerm,
                TypeName: searchTerm,
                Description: searchTerm,
                Url: searchTerm,
                Comment: searchTerm));
        }
    }
}
