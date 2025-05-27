using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Application.Services.Handlers;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Settings;
using FhirArtifactAnalyzer.Domain.Utils;
using FhirArtifactAnalyzer.Domain.Validation;
using FhirArtifactAnalyzer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped(_ => DocumentStoreHolder.Store.OpenSession());

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
