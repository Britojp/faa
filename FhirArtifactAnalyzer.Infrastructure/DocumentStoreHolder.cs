using Raven.Client.Documents;

namespace FhirArtifactAnalyzer.Infrastructure
{
    public static class DocumentStoreHolder
    {
        private static readonly Lazy<IDocumentStore> LazyStore = new(DefineDocumentStore);

        public static IDocumentStore Store => LazyStore.Value;

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
