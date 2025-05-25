using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Extensions;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Indexes;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using FhirArtifactAnalyzer.Infrastructure.Repositories.Abstractions;
using Raven.Client.Documents.Indexes;

namespace FhirArtifactAnalyzer.Infrastructure.Repositories
{
    public class FhirResourceRepository : Repository<FhirResource>, IFhirResourceRepository
    {
        public FhirResourceRepository(IRavenDBContext context) : base(context)
        {
            CreateIndexes(context);
        }

        public IEnumerable<FhirResource> Search(FhirResourceSearchParameters parameters, SearchQueryOperator @operator = SearchQueryOperator.Or)
        {
            var query = Session.Advanced.DocumentQuery<FhirResource, FhirResource_BySearchingProperties>();

            var useAndOperator = @operator == SearchQueryOperator.And;

            if (parameters.Name.HasValue())
            {
                query.Search(resource => resource.Name, parameters.Name);
                
                if (useAndOperator) query.AndAlso();
            }

            if (parameters.TypeName.HasValue())
            {
                query.Search(resource => resource.TypeName, parameters.TypeName);
                
                if (useAndOperator) query.AndAlso();
            }

            if (parameters.Description.HasValue())
            {
                query.Search(resource => resource.Description, parameters.Description);
                
                if (useAndOperator) query.AndAlso();
            }

            if (parameters.Url.HasValue())
            {
                query.Search(resource => resource.Url, parameters.Url);
                
                if (useAndOperator) query.AndAlso();
            }

            if (parameters.Comment.HasValue())
            {
                query.Search(resource => resource.Comment, parameters.Comment);
            }

            return query.ToArray();
        }

        private static void CreateIndexes(IRavenDBContext context)
        {
            var store = context.DocumentStore;

            var indexesToCreate = new List<AbstractIndexCreationTask>
            {
                new FhirResource_BySearchingProperties()
            };

            store.ExecuteIndexes(indexesToCreate, store.Database);
        }
    }
}
