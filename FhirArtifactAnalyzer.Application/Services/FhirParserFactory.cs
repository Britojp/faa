using FhirArtifactAnalyzer.Application.Parsers;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Constants;

namespace FhirArtifactAnalyzer.Application.Services
{
    public class FhirParserFactory : IFhirParserFactory
    {
        public IFhirParser GetParserForFile(string filename)
        {
            var extension = Path.GetExtension(filename);
            return GetParserForFileExtension(extension);
        }

        public IFhirParser GetParserForFileExtension(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                FileExtensions.Json => new JsonParserStrategy(),
                FileExtensions.Xml => new XmlParserStrategy(),
                _ => throw new NotSupportedException($"Extension '{extension}' is not supported!")
            };
        }
    }
}