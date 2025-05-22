using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Models;
using Nest;

namespace FhirArtifactAnalyzer.Application.Searchers
{
    public class FhirResourceSearcherWithElasticsearch : IFhirResourceSearcher
    {
        private readonly ElasticClient _client;

        public FhirResourceSearcherWithElasticsearch(ElasticClient client)
        {
            _client = client;
        }

        public IEnumerable<FhirResource> Search(
            FhirResourceSearchParameters parameters, 
            bool enableSubstringSearch = false, 
            SearchQueryOperator @operator = SearchQueryOperator.Or)
        {
            var searchRequest = null as ISearchRequest<FhirResource>;

            return _client.Search<FhirResource>(searchRequest).Documents;
        }

        public IEnumerable<FhirResource> Search(string searchTerm, bool enableSubstringSearch = false)
        {
            var searchRequest = null as ISearchRequest<FhirResource>;

            return _client.Search<FhirResource>(searchRequest).Documents;
        }
    }
}
