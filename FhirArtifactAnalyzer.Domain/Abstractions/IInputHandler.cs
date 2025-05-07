namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IInputHandler
    {
        bool CanHandle(string path);
        string Extract(string path, string destinationDirectory);
    }
}