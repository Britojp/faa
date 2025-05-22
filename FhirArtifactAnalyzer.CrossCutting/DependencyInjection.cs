using FhirArtifactAnalyzer.Application.Searchers;
using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Settings;
using FhirArtifactAnalyzer.Domain.Utils;
using FhirArtifactAnalyzer.Domain.Validation;
using FhirArtifactAnalyzer.Infrastructure;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using FhirArtifactAnalyzer.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace FhirArtifactAnalyzer.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IFhirParserFactory, FhirParserFactory>();
            services.AddScoped<IFhirResourceSearcher, FhirResourceSearcherWithDatabaseIndex>();

            services.AddScoped(_ => 
            {
                var defaultIndexName = Environment.GetEnvironmentVariable("ELASTICSEARCH_DEFAULT_INDEX_NAME")
                    ?? throw new ArgumentException("Environment variable 'ELASTICSEARCH_URI' not set.");

                var uri = Environment.GetEnvironmentVariable("ELASTICSEARCH_URI") 
                    ?? throw new ArgumentException("Environment variable 'ELASTICSEARCH_URI' not set.");
                
                var settings = new ConnectionSettings(new Uri(uri)).DefaultIndex(defaultIndexName);
                
                return new ElasticClient(settings);
            });

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IRavenDBContext, RavenDBContext>();
            services.AddScoped<IFhirResourceRepository, FhirResourceRepository>();

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
