using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FhirArtifactAnalyzer;
using FhirArtifactAnalyzer.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace FhirArtifactAnalyzer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Register HTTP client for API communication
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            // Register custom services
            builder.Services.AddScoped<FhirApiService>();
            builder.Services.AddScoped<GraphService>();
            builder.Services.AddScoped<ExportService>();

            // Configure local storage if needed
            builder.Services.AddBlazoredLocalStorage();

            await builder.Build().RunAsync();
        }
    }
}
