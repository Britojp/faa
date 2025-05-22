using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Extensions;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Application.Searchers
{
    public class FhirResourceSearcherWithDatabaseIndex : IFhirResourceSearcher
    {
        private readonly IFhirResourceRepository _resourceRepository;

        public FhirResourceSearcherWithDatabaseIndex(IFhirResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        public IEnumerable<FhirResource> Search(
            FhirResourceSearchParameters parameters, 
            bool enableSubstringSearch = false,
            SearchQueryOperator @operator = SearchQueryOperator.Or)
        {
            if (enableSubstringSearch)
            {
                parameters = parameters with
                {
                    Name = parameters.Name.WithWildcards(),
                    TypeName = parameters.TypeName.WithWildcards(),
                    Description = parameters.Description.WithWildcards(),
                    Url = parameters.Url.WithWildcards(),
                    Comment = parameters.Comment.WithWildcards(),
                };
            }

            return _resourceRepository.Search(parameters, @operator);
        }

        public IEnumerable<FhirResource> Search(string searchTerm, bool enableSubstringSearch = false)
        {
            if (enableSubstringSearch)
            {
                searchTerm = searchTerm.WithWildcards();
            }

            return _resourceRepository.Search(new FhirResourceSearchParameters(searchTerm));
        }
    }
}
