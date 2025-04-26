using FhirArtifactAnalyzer.Domain.Abstractions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirArtifactAnalyzer.Infrastructure.Services
{
    public class FhirParserService : IFhirParseService
    {
        public Resource Parse(Stream inputStream, string format)
        {
            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                return ParseJson(inputStream);
            }
            else if (format.Equals("xml", StringComparison.OrdinalIgnoreCase))
            {
                return ParseXml(inputStream);
            }
            
            throw new ArgumentException("Unsupported format");
        }

        private Resource ParseJson(Stream inputStream)
        {
            using var reader = new StreamReader(inputStream);
            string content = reader.ReadToEnd();
            var parser = new FhirJsonParser();
            return parser.Parse<Resource>(content);
        }

        private Resource ParseXml(Stream inputStream)
        {
            using var reader = new StreamReader(inputStream);
            string content = reader.ReadToEnd();
            var parser = new FhirXmlParser();
            return parser.Parse<Resource>(content);
        }
    }
}