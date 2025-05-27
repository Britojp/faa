using Raven.Client.Documents;

namespace FhirArtifactAnalyzer.Infrastructure.Interfaces
{
    public interface IRavenContext
    {
        IDocumentStore DocumentStore { get; }
    }
}
