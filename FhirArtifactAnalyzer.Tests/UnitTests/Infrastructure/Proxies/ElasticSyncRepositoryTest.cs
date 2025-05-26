using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using FhirArtifactAnalyzer.Infrastructure.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace FhirArtifactAnalyzer.Tests.UnitTests.Infrastructure.Proxies
{
    public class ElasticSyncRepositoryTest : UnitTest
    {
        private record Teste(string Id, string Name) : IEntity;

        private readonly ElasticSyncRepository<Teste> _repository;

        public ElasticSyncRepositoryTest()
        {
            var ravenContext = _serviceProvider.GetRequiredService<IRavenContext>();
            var elasticClient = _serviceProvider.GetRequiredService<IElasticClient>();

            _repository = new ElasticSyncRepository<Teste>(ravenContext, elasticClient);
        }

        [Fact]
        public void Add_ShouldIndexDocumentInBothDatabaseAndElasticSearch()
        {
            // Arrange
            var entity = new Teste("1", "Test Entity");

            // Act
            _repository.Add(entity);
            _repository.Commit();
            
            // Assert
            var indexedEntityFromDatabase = _repository.Get("1");
            var indexedEntityFromElasticSearch = _elasticFixture.Client.Get<Teste>(entity.Id);

            Assert.NotNull(indexedEntityFromDatabase);
            Assert.NotNull(indexedEntityFromElasticSearch);

            Assert.Equivalent(entity, indexedEntityFromDatabase, true);
            Assert.Equivalent(entity, indexedEntityFromElasticSearch, true);
        }

        [Fact]
        public void Delete_ShouldRemoveEntityFromElasticSearch()
        {
            // Arrange
            var entity = new Teste("3", "Entity to Delete");
            _repository.Add(entity);
            _repository.Commit();
            // Act
            _repository.Delete(entity);
            _repository.Commit();
            // Assert
            var result = _repository.Get("3");
            Assert.Null(result);
        }
    }
}
