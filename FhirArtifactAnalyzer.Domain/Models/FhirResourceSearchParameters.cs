namespace FhirArtifactAnalyzer.Domain.Models
{
    public class FhirResourceSearchParameters
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Comment { get; set; }
    }
}
