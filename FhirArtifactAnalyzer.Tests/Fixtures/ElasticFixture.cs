using FhirArtifactAnalyzer.Infrastructure.Utils;
using Nest;

namespace FhirArtifactAnalyzer.Tests.Fixtures
{
    public class ElasticFixture : IDisposable
    {
        public ElasticClient Client { get; }
        public string IndexName { get; }

        public ElasticFixture(string indexName)
        {
            IndexName = indexName;

            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(IndexName);

            Client = new ElasticClient(settings);

            ElasticSearchIndexInitializer.EnsureIndexExists(Client, IndexName);
        }

        public void Dispose()
        {
            Client.Indices.Delete(IndexName);
            GC.SuppressFinalize(this);
        }
    }

}
