namespace FhirArtifactAnalyzer.Domain.Models
{
    public record FhirResourceSearchParameters(
        string? Id,
        string? Name,
        string? TypeName,
        string? Description,
        string? Url,
        string? Comment);
}
