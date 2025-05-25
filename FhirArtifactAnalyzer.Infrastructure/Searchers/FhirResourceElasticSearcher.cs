using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Extensions;
using FhirArtifactAnalyzer.Domain.Models;
using Nest;
using System.Linq.Expressions;

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
            SearchQueryOperator @operator = SearchQueryOperator.Or,
            bool enableSubstringSearch = false)
        {
            if (enableSubstringSearch)
                parameters = parameters.WithWildcards();

            var queries = new List<Func<QueryContainerDescriptor<FhirResource>, QueryContainer>>();

            if (parameters.Name.HasValue())
                queries.Add(BuildQueryByProperty(f => f.Name, parameters.Name!, enableSubstringSearch));

            if (parameters.TypeName.HasValue())
                queries.Add(BuildQueryByProperty(f => f.TypeName, parameters.TypeName!, enableSubstringSearch));

            if (parameters.Description.HasValue())
                queries.Add(BuildQueryByProperty(f => f.Description, parameters.Description!, enableSubstringSearch));

            if (parameters.Url.HasValue())
                queries.Add(BuildQueryByProperty(f => f.Url, parameters.Url!, enableSubstringSearch));

            if (parameters.Comment.HasValue())
                queries.Add(BuildQueryByProperty(f => f.Comment, parameters.Comment!, enableSubstringSearch));

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

        private static Func<QueryContainerDescriptor<FhirResource>, QueryContainer> BuildQueryByProperty(
            Expression<Func<FhirResource, string?>> property,
            string value,
            bool enableSubstringSearch)
        {
            Func<QueryContainerDescriptor<FhirResource>, QueryContainer> query;

            if (enableSubstringSearch)
            {
                query = (q) => q
                    .Wildcard(w => w
                        .Field(property)
                        .Value(value));
            }
            else
            {
                query = (q) => q
                    .Match(m => m
                        .Field(property)
                        .Query(value));
            }

            return query;
        }
    }
}
