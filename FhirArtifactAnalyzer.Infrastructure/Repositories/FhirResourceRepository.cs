using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Indexes;
using FhirArtifactAnalyzer.Infrastructure.Repositories.Abstractions;
using Raven.Client.Documents.Session;

namespace FhirArtifactAnalyzer.Infrastructure.Repositories
{
    public class FhirResourceRepository : Repository<FhirResource>
    {
        private readonly IDocumentSession _session;

        public FhirResourceRepository(IDocumentSession session) : base(session)
        {
            _session = session;
            CreateIndexes();
        }

        public IEnumerable<FhirResource> Search(FhirResourceSearchParameters parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FhirResource> Search(string searchTerm)
        {
            var query = _session
                .Advanced
                .DocumentQuery<FhirResource, FhirResource_BySearchingProperties>()
                .Search(x => x.Name, searchTerm)
                .OrElse()
                .Search(x => x.TypeName, searchTerm)
                .OrElse()
                .Search(x => x.Description, searchTerm)
                .OrElse()
                .Search(x => x.Url, searchTerm)
                .OrElse()
                .Search(x => x.Comment, searchTerm);

            return query.ToList();
        }

        private void CreateIndexes()
        {
            new FhirResource_BySearchingProperties().Execute(_session.Advanced.DocumentStore);
        }
    }
}
