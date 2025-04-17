namespace FhirArtifactAnalyzer.Domain.Models
{
    public record AttachmentFile(string Name, Stream Stream, string ContentType);
}
