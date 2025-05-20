using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Extensions;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Indexes;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using FhirArtifactAnalyzer.Infrastructure.Repositories.Abstractions;

namespace FhirArtifactAnalyzer.Infrastructure.Repositories
{
    public class FhirResourceRepository : Repository<FhirResource>, IFhirResourceSearcherStrategy
    {
        public FhirResourceRepository(IRavenDBContext context) : base(context)
        {
            CreateIndexes();
        }

        public IEnumerable<FhirResource> Search(FhirResourceSearchParameters parameters)
        {
            var query = Session.Advanced.DocumentQuery<FhirResource, FhirResource_BySearchingProperties>();

            if (parameters.Name.HasValue())
                query.Search(x => x.Name, parameters.Name);

            if (parameters.TypeName.HasValue())
                query.Search(x => x.TypeName, parameters.TypeName);

            if (parameters.Description.HasValue())
                query.Search(x => x.Description, parameters.Description);

            if (parameters.Url.HasValue())
                query.Search(x => x.Url, parameters.Url);

            if (parameters.Comment.HasValue())
                query.Search(x => x.Comment, parameters.Comment);

            return query.ToArray();
        }

        private void CreateIndexes()
        {
            new FhirResource_BySearchingProperties().Execute(Session.Advanced.DocumentStore);
        }
    }
}
