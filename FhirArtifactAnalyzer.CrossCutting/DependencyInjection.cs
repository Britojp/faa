using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Domain.Settings;
using FhirArtifactAnalyzer.Domain.Utils;
using FhirArtifactAnalyzer.Domain.Validation;
using FhirArtifactAnalyzer.Infrastructure;
using FhirArtifactAnalyzer.Infrastructure.Configuration;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using FhirArtifactAnalyzer.Infrastructure.Proxies;
using FhirArtifactAnalyzer.Infrastructure.Searchers;
using FhirArtifactAnalyzer.Infrastructure.Utils;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace FhirArtifactAnalyzer.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IFhirParserFactory, FhirParserFactory>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(_ =>
            {
                var uri = new Uri(ElasticSearchConfiguration.Uri);
                var settings = new ConnectionSettings(uri)
                    .DefaultIndex(ElasticSearchConfiguration.DefaultIndexName);

                var client = new ElasticClient(settings);
                ElasticSearchInitializer.EnsureIndexExists(client);

                return client;
            });

            services.AddScoped<IRavenDBContext, RavenDBContext>();
            services.AddScoped<Domain.Abstractions.IRepository<FhirResource>, FhirResourceElasticSyncRepository>();
            services.AddScoped<IFhirResourceSearcher, FhirResourceElasticSearcher>();

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IFhirValidator, LocalFhirValidator>();

            return services;
        }

        public static IServiceCollection ConfigureGlobalOptions(this IServiceCollection services)
        {
            var solutionDir = SolutionPathFinder.GetSolutionDirectory();
            var workingDir = Path.Combine(solutionDir, "FhirArtifactAnalyzer.ValidationArtifacts");

            services.Configure<ArtifactValidationSettings>(opts =>
            {
                opts.WorkingDirectory = workingDir;
            });

            return services;
        }
    }
}
