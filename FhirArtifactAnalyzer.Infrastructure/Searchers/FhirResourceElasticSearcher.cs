using Elasticsearch.Net;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Extensions;
using FhirArtifactAnalyzer.Domain.Models;
using Nest;

namespace FhirArtifactAnalyzer.Infrastructure.Searchers
{
    public class FhirResourceElasticSearcher : IFhirResourceSearcher
    {
        private readonly ElasticClient _client;

        public FhirResourceElasticSearcher(ElasticClient client)
        {
            _client = client;
        }

        public IEnumerable<FhirResource> Search(
            FhirResourceSearchParameters parameters,
            bool enableSubstringSearch = false,
            SearchQueryOperator @operator = SearchQueryOperator.Or)
        {
            if (enableSubstringSearch)
                parameters = parameters.WithWildcards();

            var queries = new List<Func<QueryContainerDescriptor<FhirResource>, QueryContainer>>();

            if (parameters.Name.HasValue())
            {
                queries.Add(q =>
                    enableSubstringSearch
                    ? q.Wildcard(w => w
                        .Field(f => f.Name)
                        .Value(parameters.Name))
                    : q.Match(m => m
                        .Field(f => f.Name)
                        .Query(parameters.Name)));
            }

            if (parameters.TypeName.HasValue())
            {
                queries.Add(q =>
                    enableSubstringSearch
                    ? q.Wildcard(w => w
                        .Field(f => f.TypeName)
                        .Value(parameters.TypeName))
                    : q.Match(m => m
                        .Field(f => f.TypeName)
                        .Query(parameters.TypeName)));
            }

            if (parameters.Description.HasValue())
            {
                queries.Add(q =>
                    enableSubstringSearch
                    ? q.Wildcard(w => w
                        .Field(f => f.Description)
                        .Value(parameters.Description))
                    : q.Match(m => m
                        .Field(f => f.Description)
                        .Query(parameters.Description)));
            }

            if (parameters.Url.HasValue())
            {
                queries.Add(q =>
                    enableSubstringSearch
                    ? q.Wildcard(w => w
                        .Field(f => f.Url)
                        .Value(parameters.Url))
                    : q.Match(m => m
                        .Field(f => f.Url)
                        .Query(parameters.Url)));
            }

            if (parameters.Comment.HasValue())
            {
                queries.Add(q =>
                    enableSubstringSearch
                    ? q.Wildcard(w => w
                        .Field(f => f.Comment)
                        .Value(parameters.Comment))
                    : q.Match(m => m
                        .Field(f => f.Comment)
                        .Query(parameters.Comment)));
            }

            var searchRequest = new SearchDescriptor<FhirResource>()
                .Query(q => q
                    .Bool(b => @operator == SearchQueryOperator.Or
                        ? b.Should(queries)
                        : b.Must(queries)));

            return Search(searchRequest);
        }

        public IEnumerable<FhirResource> Search(string searchTerm, bool enableSubstringSearch = false)
        {
            if (enableSubstringSearch)
                searchTerm = searchTerm.WithWildcards();

            var searchRequest = new SearchDescriptor<FhirResource>()
                .Query(q => q
                    .QueryString(qs => qs
                        .Fields(f => f
                            .Field(r => r.Name)
                            .Field(r => r.TypeName)
                            .Field(r => r.Description)
                            .Field(r => r.Url)
                            .Field(r => r.Comment))
                        .Query(searchTerm)
                        .DefaultOperator(Operator.Or)
                    )
                );

            return Search(searchRequest);
        }

        private IEnumerable<FhirResource> Search(ISearchRequest searchRequest)
        {
            var searchResponse = _client.Search<FhirResource>(searchRequest);

            if (!searchResponse.IsValid)
            {
                throw new Exception($"Search failed: {searchResponse.ServerError.Error.Reason}");
            }

            return searchResponse.Documents;
        }
    }
}
