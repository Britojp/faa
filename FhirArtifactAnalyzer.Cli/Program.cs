using FhirArtifactAnalyzer.CrossCutting;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using System.Net.Mime;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        using var host = CreateHost(args);
        using var scope = CreateScope(host);

        var searcher = scope.ServiceProvider.GetRequiredService<IFhirResourceSearcher>();

        var path = "C:\\Users\\Iglesias\\source\\repos\\FhirArtifactAnalyzer\\FhirArtifactAnalyzer.Cli\\" +
            "OperationDefinition.json";

        //var path = @"C:\Users\Usuario\source\repos\My-Projects\faa\FhirArtifactAnalyzer.Cli\OperationDefinition.json";

        var conteudoJson = File.ReadAllText(path);
        var instance = new FhirJsonParser().Parse<OperationDefinition>(conteudoJson);

        byte[] bytes = Encoding.UTF8.GetBytes(conteudoJson);

        var memoryStream = new MemoryStream(bytes)
        {
            Position = 0
        };

        var teste = new FhirResource()
        {
            Id = instance.Id,
            Name = instance.Name,
            Description = instance.Description,
            Url = instance.Url,
            Comment = instance.Comment,
            TypeName = instance.TypeName,
            Attachment = new(instance.Id + ".json", MediaTypeNames.Application.Json)

        };

        var buscado = searcher.Search(new FhirResourceSearchParameters(null, "LAE", null, null, null, "fa"), true, @operator: SearchQueryOperator.Or).FirstOrDefault();
        
        var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .DefaultIndex("recursos");

        var client = new ElasticClient(settings);

        // 1. Indexar um documento
        var indexResponse = client.IndexDocument(teste);
        Console.WriteLine("Indexado? " + indexResponse.IsValid);

        // 2. Buscar por título
        var searchResponse = client.Search<FhirResource>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Comment)
                    .Query("labs")
                )
            )
        );
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
