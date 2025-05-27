namespace FhirArtifactAnalyzer.Infrastructure.Configuration
{
    public class ElasticSearchConfiguration
    {
        public static readonly string DefaultIndexName
            = Environment.GetEnvironmentVariable("ELASTICSEARCH_DEFAULT_INDEX_NAME")
                    ?? throw new ArgumentException("Environment variable 'ELASTICSEARCH_URI' not set.");

        public static readonly string Uri
            = Environment.GetEnvironmentVariable("ELASTICSEARCH_URI")
                ?? throw new ArgumentException("Environment variable 'ELASTICSEARCH_URI' not set.");
    }
}
