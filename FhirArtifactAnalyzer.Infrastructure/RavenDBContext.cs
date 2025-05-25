using FhirArtifactAnalyzer.Infrastructure.Configuration;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace FhirArtifactAnalyzer.Infrastructure
{
    public class RavenDBContext : IRavenDBContext
    {
        private static readonly Lazy<IDocumentStore> _lazyStore = new(DefineDocumentStore);

        public IDocumentStore DocumentStore => _lazyStore.Value;

        public IDocumentSession OpenSession() => DocumentStore.OpenSession();

        private static IDocumentStore DefineDocumentStore()
        {
            var store = new DocumentStore
            {
                Urls = [RavenConfiguration.Url],
                Database = RavenConfiguration.DatabaseName,
                Certificate = RavenConfiguration.Certificate,
            };

            return store.Initialize();
        }
    }
}
