using FhirArtifactAnalyzer.Domain.Abstractions;

namespace FhirArtifactAnalyzer.Domain.Models
{
    public class FhirResource : IEntity
    {
        public required string Id { get; init; }
        public required string Name { get; set; }
        public required string TypeName { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
        public string? Comment { get; set; }

        public required AttachmentInfo Attachment { get; init; }

        public record AttachmentInfo(string Name, string ContentType);
    }
}
