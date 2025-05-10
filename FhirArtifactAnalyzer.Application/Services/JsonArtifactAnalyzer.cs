using FhirArtifactAnalyzer.Domain.Abstractions;
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

        private static readonly HashSet<string> RelevantTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(StructureDefinition),
            nameof(CodeSystem),
            nameof(ValueSet),
            nameof(CapabilityStatement),
            nameof(ImplementationGuide),
            nameof(OperationDefinition),
            nameof(SearchParameter)
        };

        public FhirArtifactInfo Analyze(FhirArtifactInfo artifact)
        {
            if (!File.Exists(artifact.FilePath))
            {
                artifact.ReasonIgnored = "Arquivo não encontrado.";
                return artifact;
            }

            if (!artifact.IsJson)
            {
                artifact.ReasonIgnored = "Extensão não é .json.";
                return artifact;
            }

            var fileInfo = new FileInfo(artifact.FilePath);
            if (fileInfo.Length > _maxSize)
            {
                artifact.ReasonIgnored = "Arquivo excede o tamanho máximo permitido.";
                return artifact;
            }

            try
            {
                var content = File.ReadAllText(artifact.FilePath);

                using var doc = JsonDocument.Parse(content);

                artifact.IsWellFormedJson = true;

                if (!doc.RootElement.TryGetProperty("resourceType", out var resourceTypeElement))
                {
                    artifact.ReasonIgnored = "Campo 'resourceType' não encontrado.";
                    return artifact;
                }
                
                var resourceType = resourceTypeElement.GetString();
                artifact.ResourceType = resourceType;

                if (!RelevantTypes.Contains(resourceType))
                {
                    artifact.ReasonIgnored = $"resourceType '{resourceType}' não é relevante.";
                    return artifact;
                }

                artifact.IsRelevantFhirResource = true;
            }
            catch (IOException ex)
            {
                artifact.ReasonIgnored = $"Erro ao ler o arquivo: {ex.Message}";
            }
            catch (JsonException)
            {
                artifact.IsWellFormedJson = false;
                artifact.ReasonIgnored = "JSON malformado.";
            }
            
            return artifact;
        }
    }
}