using FhirArtifactAnalyzer.CrossCutting;
using FhirArtifactAnalyzer.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents.Session;

public record Teste(string? Id, string Name);

class Program
{
    public static void Main(string[] args)
    {
        using var host = CreateHost(args);
        using var scope = CreateScope(host);
        
        Console.WriteLine("Hello Fhir!");
    }

    private static IHost CreateHost(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructure();
            })
            .Build();
    }

    private static IServiceScope CreateScope(IHost host)
    {
        return host.Services.CreateScope();
    }
}
