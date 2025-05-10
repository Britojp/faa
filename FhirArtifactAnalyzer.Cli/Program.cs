using FhirArtifactAnalyzer.CrossCutting;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using System.Text.Json;

class Program
{
    public static void Main(string[] args)
    {
        using var host = CreateHost(args);
        using var scope = CreateScope(host);

        var path = "C:\\Users\\Iglesias\\source\\repos\\FhirArtifactAnalyzer\\FhirArtifactAnalyzer.Cli\\OperationDefinition.json";
        var conteudoJson = File.ReadAllText(path);
        var parser = new FhirJsonParser();
        var instance1 = parser.Parse<ValueSet>(conteudoJson).Name;
        var instance2 = parser.Parse<ValueSet>(conteudoJson).Url;
        var instance4 = parser.Parse<ValueSet>(conteudoJson).Description;

        var instance5 = parser.Parse<ValueSet>(conteudoJson).TypeName;

        var instance = parser.Parse<ValueSet>(conteudoJson);
        
        Console.WriteLine("Hello Fhir!");
    }

    private static IHost CreateHost(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddApplicationServices();
                services.AddInfrastructure();
                services.AddDomainServices();
                services.ConfigureGlobalOptions();
            })
            .Build();
    }

    private static IServiceScope CreateScope(IHost host)
    {
        return host.Services.CreateScope();
    }
}
