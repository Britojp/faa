using FhirArtifactAnalyzer.Infrastructure.Configuration;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using Raven.Client.Documents;

namespace FhirArtifactAnalyzer.Infrastructure
{
    public class RavenContext : IRavenContext
    {
        private static readonly Lazy<IDocumentStore> _lazyStore = new(DefineDocumentStore);

        public IDocumentStore DocumentStore => _lazyStore.Value;

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
