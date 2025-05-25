using System.Security.Cryptography.X509Certificates;

namespace FhirArtifactAnalyzer.Infrastructure.Configuration
{
    public static class RavenConfiguration
    {
        public static readonly string DatabaseName
            = Environment.GetEnvironmentVariable("RAVENDB_DATABASE_NAME")
                ?? throw new ArgumentException("Environment variable 'RAVENDB_DATABASE_NAME' not set.");
        
        public static readonly string Url
            = Environment.GetEnvironmentVariable("RAVENDB_URL")
                ?? throw new ArgumentException("Environment variable 'RAVENDB_URL' not set.");

        public static readonly X509Certificate2? Certificate = null;
    }
}
