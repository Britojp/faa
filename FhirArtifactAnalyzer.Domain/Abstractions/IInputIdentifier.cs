using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IInputIdentifier
    {
        public InputType? GetInputType(string pathOrUrl);
    }
}
