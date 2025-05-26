using FhirArtifactAnalyzer.CrossCutting;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using FhirArtifactAnalyzer.Tests.Fixtures;
using FhirArtifactAnalyzer.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Raven.TestDriver;

namespace FhirArtifactAnalyzer.Tests.UnitTests
{
    public abstract class UnitTest : RavenTestDriver
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly RavenContextMock _ravenContext;
        protected readonly ElasticFixture _elasticFixture;

        public UnitTest()
        {
            _serviceProvider = GetServiceCollection().BuildServiceProvider();
            _ravenContext = new RavenContextMock(GetDocumentStore());
            _elasticFixture = new ElasticFixture("test-index");
        }

        private ServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddApplicationServices();
            services.AddInfrastructure();
            services.AddDomainServices();
            services.ConfigureGlobalOptions();

            services.AddScoped<IElasticClient>(_ => _elasticFixture.Client);
            services.AddScoped<IRavenContext>(_ => _ravenContext);

            return services;
        }
    }
}
