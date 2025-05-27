using FhirArtifactAnalyzer.Domain.Extensions;

namespace FhirArtifactAnalyzer.Domain.Models
{
    public record FhirResourceSearchParameters
    {
        private string? _name;
        private string? _typeName;
        private string? _description;
        private string? _url;
        private string? _comment;

        public string? Name { get => _name; set => _name = value?.ToLower(); }
        public string? TypeName { get => _typeName; set => _typeName = value?.ToLower(); }
        public string? Description { get => _description; set => _description = value?.ToLower(); }
        public string? Url { get => _url; set => _url = value?.ToLower(); }
        public string? Comment { get => _comment; set => _comment = value?.ToLower(); }

        public FhirResourceSearchParameters() { }

        public FhirResourceSearchParameters(string searchTerm)
        {
            ArgumentException.ThrowIfNullOrEmpty(searchTerm, nameof(searchTerm));

            Name = searchTerm;
            TypeName = searchTerm;
            Description = searchTerm;
            Url = searchTerm;
            Comment = searchTerm;
        }

        public FhirResourceSearchParameters WithWildcards()
        {
            return new FhirResourceSearchParameters
            {
                Name = Name?.WithWildcards(),
                TypeName = TypeName?.WithWildcards(),
                Description = Description?.WithWildcards(),
                Url = Url?.WithWildcards(),
                Comment = Comment?.WithWildcards()
            };
        }
    }
}
