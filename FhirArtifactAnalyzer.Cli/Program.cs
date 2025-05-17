using FhirArtifactAnalyzer.CrossCutting;
using FhirArtifactAnalyzer.Domain.Constants;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Indexes;
using FhirArtifactAnalyzer.Infrastructure.Repositories;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries.Timings;
using Raven.Client.Documents.Session;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

class Program
{
    public static void Main(string[] args)
    {
        using var host = CreateHost(args);
        using var scope = CreateScope(host);

        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();
        var repository = new Repository<object>(session);

        var path = "C:\\Users\\Iglesias\\source\\repos\\FhirArtifactAnalyzer\\FhirArtifactAnalyzer.Cli\\" +
            "OperationDefinition.json";

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
        new FhirResource_BySearchingProperties().Execute(session.Advanced.DocumentStore);
        var buscado = session.Advanced
            .DocumentQuery<FhirResource, FhirResource_BySearchingProperties>()
            .Search(x => x.Comment, "labs")
            .SelectFields<FhirResource>()
            .FirstOrDefault();

        var a = session.Load<FhirResource>(buscado.Id);

        var aa = session.Advanced.Attachments.Get(a, a.Attachment.Name);

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
