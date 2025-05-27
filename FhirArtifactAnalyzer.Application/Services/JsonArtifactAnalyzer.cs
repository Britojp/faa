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
    public class JsonArtifactAnalyzer : IArtifactAnalyzer
    {
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

        /// <summary>
        /// Inicializa o servi�o com a lista de manipuladores de entrada disponiveis.
        /// </summary>
        public async Task<FhirArtifactInfo> AnalyzeAsync(FhirArtifactInfo artifact)
        {
            if (!File.Exists(artifact.FilePath))
                return Ignore(artifact, "Arquivo não encontrado.");

            if (!IsJsonFile(artifact.FilePath))
            {
                artifact.IsJson = false;
                return Ignore(artifact, "Extensão não é .json.");
            }

            var fileInfo = new FileInfo(artifact.FilePath);
            if (fileInfo.Length > _maxSize)
                return Ignore(artifact, "Arquivo excede o tamanho máximo permitido.");

            try
            {
                var content = await File.ReadAllTextAsync(artifact.FilePath);

                using var doc = JsonDocument.Parse(content);
                artifact.IsWellFormedJson = true;

                if (!doc.RootElement.TryGetProperty("resourceType", out var resourceTypeElement))
                    return Ignore(artifact, "Campo 'resourceType' não encontrado.");

                var resourceType = resourceTypeElement.GetString();
                artifact.ResourceType = resourceType;

                if (!RelevantResourceTypes.Contains(resourceType))
                    return Ignore(artifact, $"resourceType '{resourceType}' não é relevante.");

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
            catch (JsonException)
            {
                artifact.IsWellFormedJson = false;
                return Ignore(artifact, "JSON malformado.");
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