using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace FhirArtifactAnalyzer.Infrastructure.Interfaces
{
    public interface IRavenDBContext
    {
        IDocumentStore DocumentStore { get; }
        IDocumentSession OpenSession();
    }
}
