using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace FhirArtifactAnalyzer.Infrastructure
{
    public class RavenDBContext : IRavenDBContext
    {
        private static readonly Lazy<IDocumentStore> _lazyStore = new(DefineDocumentStore);

        public IDocumentStore DocumentStore => _lazyStore.Value;

        public IDocumentSession OpenSession()
        {
            return DocumentStore.OpenSession();
        }

        private static IDocumentStore DefineDocumentStore()
        {
            var databaseName = Environment.GetEnvironmentVariable("RAVENDB_DATABASE_NAME");
            var databaseUrl = Environment.GetEnvironmentVariable("RAVENDB_URL");

            var store = new DocumentStore
            {
                Urls = [databaseUrl],
                Database = databaseName,
                Certificate = null,
            };

            return store.Initialize();
        }
    }
}
