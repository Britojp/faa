using FhirArtifactAnalyzer.CrossCutting;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Enums;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Mime;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        using var host = CreateHost(args);
        using var scope = CreateScope(host);

        var dbContext = scope.ServiceProvider.GetRequiredService<IRavenDBContext>();
        var searcher = scope.ServiceProvider.GetRequiredService<IFhirResourceSearcher>();
        var repo = scope.ServiceProvider.GetRequiredService<IFhirResourceSearcher>();
        using var session = dbContext.OpenSession();

        //var path = "C:\\Users\\Iglesias\\source\\repos\\FhirArtifactAnalyzer\\FhirArtifactAnalyzer.Cli\\" +
        //    "OperationDefinition.json";

        var path = @"C:\Users\Usuario\source\repos\My-Projects\faa\FhirArtifactAnalyzer.Cli\OperationDefinition.json";

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

        //session.Store(teste);
        //session.Advanced.Attachments.Store(teste, teste.Attachment.Name, memoryStream, MediaTypeNames.Application.Json);
        //session.SaveChanges();
        var buscado = searcher.Search(new FhirResourceSearchParameters(null, "LAE", null, null, null, "fa"), true, @operator: SearchQueryOperator.Or).FirstOrDefault();

        
        using var reader = new StreamReader(aa.Stream, Encoding.UTF8);
        string jsonString = reader.ReadToEnd();
        var resource = new FhirJsonParser().Parse<OperationDefinition>(jsonString);
        //session.Store(teste);
        //session.Advanced.Attachments.Store(teste, $"{instance.TypeName}/{instance.Id}.json", memoryStream, "application/json");
        //session.SaveChanges();

        //session.Store(instance);
        //session.SaveChanges();
        //session.Advanced.Clear();

        //var operationDefinition = session.Load<dynamic>(instance.Id);

        //Console.WriteLine("Hello Fhir!");

        //var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        //    .DefaultIndex("recursos");

        //var client = new ElasticClient(settings);

        //// 1. Indexar um documento
        //teste.Id = "2";
        //var indexResponse = client.IndexDocument(teste);
        //Console.WriteLine("Indexado? " + indexResponse.IsValid);

        //// 2. Buscar por título
        //var searchResponse = client.Search<FhirResource>(s => s
        //    .Query(q => q
        //        .Match(m => m
        //            .Field(f => f.Comment)
        //            .Query("labs")
        //        )
        //    )
        //);
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
