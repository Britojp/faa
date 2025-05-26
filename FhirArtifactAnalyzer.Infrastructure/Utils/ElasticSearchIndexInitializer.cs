using FhirArtifactAnalyzer.Domain.Models;
using Nest;

namespace FhirArtifactAnalyzer.Infrastructure.Utils
{
    public static class ElasticSearchIndexInitializer
    {
        public static void EnsureIndexExists(IElasticClient client, string indexName)
        {
            var indexExistsResponse = client.Indices.Exists(indexName);
            
            if (!indexExistsResponse.Exists)
            {
                var createIndexResponse = client.Indices.Create(
                    indexName, 
                    c => c.Map<FhirResource>(m => m.AutoMap()));

                if (!createIndexResponse.IsValid)
                {
                    throw new Exception("Failed to create index: " + createIndexResponse.DebugInformation);
                }
            }
        }
    }
}
