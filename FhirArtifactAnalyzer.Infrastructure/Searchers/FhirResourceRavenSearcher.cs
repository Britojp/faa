using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Extensions;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Extensions;
using FhirArtifactAnalyzer.Infrastructure.Indexes;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;

namespace FhirArtifactAnalyzer.Infrastructure.Searchers
{
    public class FhirResourceRavenSearcher : IFhirResourceSearcher
    {
        private readonly IRavenContext _dbContext;

        public FhirResourceRavenSearcher(IRavenContext dbContext)
        {
            _dbContext = dbContext;
            CreateIndex();
        }

        public IEnumerable<FhirResource> Search(
            FhirResourceSearchParameters parameters, 
            SearchQueryOperator @operator = SearchQueryOperator.Or,
            bool enableSubstringSearch = false)
        {
            if (enableSubstringSearch)
                parameters = parameters.WithWildcards();

            return SearchInDatabase(parameters, @operator);
        }

        public IEnumerable<FhirResource> Search(string searchTerm, bool enableSubstringSearch = false)
        {
            if (enableSubstringSearch)
                searchTerm = searchTerm.WithWildcards();

            return SearchInDatabase(new FhirResourceSearchParameters(searchTerm), SearchQueryOperator.Or);
        }

        private FhirResource[] SearchInDatabase(
            FhirResourceSearchParameters parameters, 
            SearchQueryOperator @operator)
        {
            using var session = _dbContext
                .DocumentStore
                .OpenSession();

            var query = session
                .Advanced
                .DocumentQuery<FhirResource, FhirResource_BySearchingProperties>()
                .NoTracking();

            if (parameters.Name.HasValue())
            {
                query
                    .Search(resource => resource.Name, parameters.Name)
                    .UseOperator(@operator);
            }

            if (parameters.TypeName.HasValue())
            {
                query
                    .Search(resource => resource.TypeName, parameters.TypeName)
                    .UseOperator(@operator);
            }

            if (parameters.Description.HasValue())
            {
                query
                    .Search(resource => resource.Description, parameters.Description)
                    .UseOperator(@operator);
            }

            if (parameters.Url.HasValue())
            {
                query
                    .Search(resource => resource.Url, parameters.Url)
                    .UseOperator(@operator);
            }

            if (parameters.Comment.HasValue())
            {
                query.Search(resource => resource.Comment, parameters.Comment);
            }

            return query.SelectFields<FhirResource>().ToArray();
        }

        private void CreateIndex()
        {
            new FhirResource_BySearchingProperties().Execute(_dbContext.DocumentStore);
        }
    }
}
