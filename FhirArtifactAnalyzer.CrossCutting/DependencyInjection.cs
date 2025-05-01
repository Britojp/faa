using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Infrastructure;
using FhirArtifactAnalyzer.Infrastructure.Utils;
using FhirArtifactAnalyzer.Infrastructure.Services;
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
            
            services.AddSingleton<IFhirValidatorService>(
                provider => new LocalFhirValidatorService(workingDir));

            return services;
        }
    }
}
