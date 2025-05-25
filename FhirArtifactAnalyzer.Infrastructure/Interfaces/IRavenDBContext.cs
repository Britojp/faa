using Raven.Client.Documents;

namespace FhirArtifactAnalyzer.Infrastructure.Interfaces
{
    public interface IRavenDBContext
    {
        IDocumentStore DocumentStore { get; }
    }
}
