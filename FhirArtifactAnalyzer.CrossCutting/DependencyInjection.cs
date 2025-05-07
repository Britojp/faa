using FhirArtifactAnalyzer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FhirArtifactAnalyzer.CrossCutting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped((provider) => DocumentStoreHolder.Store.OpenSession());

            return services;
        }
    }
}
