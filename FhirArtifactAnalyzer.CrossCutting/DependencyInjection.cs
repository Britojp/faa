using FhirArtifactAnalyzer.Application.Parsers;
using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Settings;
using FhirArtifactAnalyzer.Domain.Validation;
using FhirArtifactAnalyzer.Infrastructure;
using FhirArtifactAnalyzer.Infrastructure.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace FhirArtifactAnalyzer.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped((provider) => DocumentStoreHolder.Store.OpenSession());
            
            var solutionDir = SolutionPathHelper.GetSolutionDirectory();
            var workingDir = Path.Combine(Path.Combine(solutionDir, "FhirArtifactAnalyzer.ValidationArtifacts"));

            services.Configure<ArtifactValidationSettings>(opts =>
            {
                opts.WorkingDirectory = workingDir;
            });

            services.AddScoped<JsonFhirParser>();
            services.AddScoped<XmlFhirParser>();

            services.AddScoped<IFhirParserFactory, FhirParserFactory>();
            services.AddScoped<IFhirValidatorService, LocalFhirValidatorService>();

            return services;
        }
    }
}
