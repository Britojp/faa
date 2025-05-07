using FhirArtifactAnalyzer.Application.Parsers;
using FhirArtifactAnalyzer.Domain.Abstractions;

namespace FhirArtifactAnalyzer.Application.Services
{
    public class FhirParserFactory : IFhirParserFactory
    {
        private readonly IFhirParser _jsonParser;
        private readonly IFhirParser _xmlParser;

        public FhirParserFactory(JsonFhirParser jsonParser, XmlFhirParser xmlParser)
        {
            _jsonParser = jsonParser;
            _xmlParser = xmlParser;
        }

        public IFhirParser GetParserForFile(string filename)
        {
            var extension = Path.GetExtension(filename).ToLowerInvariant();

            return extension switch
            {
                ".json" => _jsonParser,
                ".xml" => _xmlParser,
                _ =>throw new NotSupportedException($"Don't support extension: {extension}")
            };
        }
    }
}