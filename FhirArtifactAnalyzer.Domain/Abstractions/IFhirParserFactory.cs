namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IFhirParserFactory
    {
        IFhirParser GetParserForFile(string filename);
        IFhirParser GetParserForFileExtension(string fileExtension);
    }
}