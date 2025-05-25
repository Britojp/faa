using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Configuration;
using Nest;

namespace FhirArtifactAnalyzer.Infrastructure.Utils
{
    public static class ElasticSearchInitializer
    {
        public static void EnsureIndexExists(ElasticClient client)
        {
            if (!client.Indices.Exists(ElasticSearchConfiguration.DefaultIndexName).Exists)
            {
                var createIndexResponse = client.Indices.Create(ElasticSearchConfiguration.DefaultIndexName, c => c
                    .Map<FhirResource>(m => m
                        .AutoMap()
                    ));

                if (!createIndexResponse.IsValid)
                {
                    throw new Exception("Failed to create index: " + createIndexResponse.DebugInformation);
                }
            }
        }
    }
}
