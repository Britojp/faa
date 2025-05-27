using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Constants;
using FhirArtifactAnalyzer.Domain.Models;
using Hl7.Fhir.Model;
using System.Text.Json;

namespace FhirArtifactAnalyzer.Application.Services
{
    /// <summary>
    /// Responsável por analisar arquivos JSON e verificar se contêm recursos FHIR relevantes.
    /// </summary>
    public class JsonArtifactAnalyzer : IJsonArtifactAnalyzer
    {
        private readonly IFhirParserFactory _parserFactory;
        private const long _maxSize = 10 * 1024 * 1024; // 10MB

        private static readonly HashSet<string> RelevantResourceTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(StructureDefinition),
            nameof(CodeSystem),
            nameof(ValueSet),
            nameof(CapabilityStatement),
            nameof(ImplementationGuide),
            nameof(OperationDefinition),
            nameof(SearchParameter)
        };
        public JsonArtifactAnalyzer(IFhirParserFactory parserFactory)
        {
            _parserFactory = parserFactory;
        }

        /// <summary>
        /// Inicializa o servi�o com a lista de manipuladores de entrada disponiveis.
        /// </summary>
        public async Task<FhirArtifactInfo> AnalyzeAsync(string filePath, InputSource source)
        {
            var artifact = new FhirArtifactInfo
            {
                FilePath = filePath,
                Source = source
            };

            if (!File.Exists(filePath))
                return Ignore(artifact, "Arquivo não encontrado.");

            if (!IsJsonFile(filePath))
            {
                artifact.IsJson = false;
                return Ignore(artifact, "Extensão não é .json.");
            }

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > _maxSize)
                return Ignore(artifact, "Arquivo excede o tamanho máximo permitido.");

            try
            {
                var content = await File.ReadAllTextAsync(filePath);
                artifact.IsWellFormedJson = true;

                var parser = _parserFactory.GetParserForFile(filePath);
                var resource = parser.Parse<Resource>(content); 

                artifact.ResourceType = resource?.TypeName;

                if (resource == null)
                    return Ignore(artifact, "Não foi possível fazer parse do recurso.");

                if (!RelevantResourceTypes.Contains(resource.TypeName))
                    return Ignore(artifact, $"resourceType '{resource.TypeName}' não é relevante.");

                artifact.IsRelevantFhirResource = true;
            }
            catch (UnauthorizedAccessException)
            {
                return Ignore(artifact, "Permissão negada ao acessar o arquivo.");
            }
            catch (IOException ex)
            {
                return Ignore(artifact, $"Erro ao ler o arquivo: {ex.Message}");
            }
            catch (Exception ex)
            {
                artifact.IsWellFormedJson = false;
                return Ignore(artifact, $"Erro ao analisar JSON: {ex.Message}");
            }

            return artifact;
        }


        private static bool IsJsonFile(string filePath) =>
            Path.GetExtension(filePath).Equals(FileExtensions.Json, StringComparison.OrdinalIgnoreCase);

        private static FhirArtifactInfo Ignore(FhirArtifactInfo artifact, string reason)
        {
            artifact.ReasonIgnored = reason;
            return artifact;
        }
    }
}