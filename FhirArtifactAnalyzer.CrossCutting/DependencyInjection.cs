using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Application.Services.Handlers;
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
            services.AddScoped<IArtifactProcessor, ArtifactProcessor>();
            services.AddScoped<IDirectoryNavigator, DirectoryNavigator>();
            services.AddScoped<IExtractorService, ExtractorService>();
            services.AddScoped<IJsonArtifactAnalyzer, JsonArtifactAnalyzer>();
            services.AddScoped<IInputIdentifier, InputIdentifier>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IElasticClient>(_ =>
            {
                var uri = new Uri(ElasticSearchConfiguration.Uri);
                var settings = new ConnectionSettings(uri)
                    .DefaultIndex(ElasticSearchConfiguration.DefaultIndexName);

                var client = new ElasticClient(settings);
                ElasticSearchIndexInitializer.EnsureIndexExists(client, ElasticSearchConfiguration.DefaultIndexName);

                return client;
            });

            services.AddScoped<IRavenContext, RavenContext>();
            services.AddScoped<Domain.Abstractions.IRepository<FhirResource>, ElasticSyncRepository<FhirResource>>();
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
