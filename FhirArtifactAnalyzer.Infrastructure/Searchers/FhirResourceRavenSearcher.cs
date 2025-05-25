using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Extensions;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Infrastructure.Searchers
{
    public class FhirResourceRavenSearcher : IFhirResourceSearcher
    {
        private readonly IFhirResourceRepository _resourceRepository;

        public FhirResourceRavenSearcher(IFhirResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        public IEnumerable<FhirResource> Search(
            FhirResourceSearchParameters parameters, 
            bool enableSubstringSearch = false,
            SearchQueryOperator @operator = SearchQueryOperator.Or)
        {
            if (enableSubstringSearch)
                parameters = parameters.WithWildcards();

            return _resourceRepository.Search(parameters, @operator);
        }

        public IEnumerable<FhirResource> Search(string searchTerm, bool enableSubstringSearch = false)
        {
            if (enableSubstringSearch)
                searchTerm = searchTerm.WithWildcards();

            return _resourceRepository.Search(new FhirResourceSearchParameters(searchTerm));
        }
    }
}
