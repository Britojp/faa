using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using Raven.Client.Documents;

namespace FhirArtifactAnalyzer.Tests.Mocks
{
    public class RavenContextMock(IDocumentStore store) : IRavenContext
    {
        public IDocumentStore DocumentStore { get; } = store;
    }
}
