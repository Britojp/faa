using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using System.Text.Json;

namespace FhirArtifactAnalyzer.Application.Services
{
    /// <summary>
    /// Responsável por analisar arquivos JSON e verificar se contêm recursos FHIR relevantes.
    /// </summary>
    public class JsonArtifactReader : IArtifactReader
    {
        private const long MaxSize = 10 * 1024 * 1024; // 10MB

        private static readonly HashSet<string> RelevantTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "StructureDefinition",
            "CodeSystem",
            "ValueSet",
            "CapabilityStatement",
            "ImplementationGuide",
            "OperationDefinition",
            "SearchParameter"
        };

        public FhirArtifactInfo Analyze(FhirArtifactInfo artifact)
        {
            artifact.IsJson = IsJsonFile(artifact.FilePath);

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
            if (fileInfo.Length > MaxSize)
            {
                artifact.ReasonIgnored = "Arquivo excede o tamanho máximo permitido.";
                return artifact;
            }

            string content;
            try
            {
                content = File.ReadAllText(artifact.FilePath);
            }
            catch (IOException ex)
            {
                artifact.ReasonIgnored = $"Erro ao ler o arquivo: {ex.Message}";
                return artifact;
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                artifact.IsWellFormedJson = true;

                if (doc.RootElement.TryGetProperty("resourceType", out var resourceTypeElement))
                {
                    var resourceType = resourceTypeElement.GetString();
                    artifact.ResourceType = resourceType;

                    if (RelevantTypes.Contains(resourceType))
                    {
                        artifact.IsRelevantFhirResource = true;
                    }
                    else
                    {
                        artifact.ReasonIgnored = $"resourceType '{resourceType}' não é relevante.";
                    }
                }
                else
                {
                    artifact.ReasonIgnored = "Campo 'resourceType' não encontrado.";
                }
            }
            catch (JsonException)
            {
                artifact.IsWellFormedJson = false;
                artifact.ReasonIgnored = "JSON malformado.";
            }

            return artifact;
        }

        private static bool IsJsonFile(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
