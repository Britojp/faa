using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Application.Services
{
    /// <summary>
    /// Responsavel pela identificação do tipo de entrada.
    /// </summary>
    public class InputIdentifier : IInputIdentifier
    {
        public IEnumerable<InputType> GetInputType(string pathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(pathOrUrl))
                yield break;

            if (Uri.IsWellFormedUriString(pathOrUrl, UriKind.Absolute))
            {
                yield return InputType.Url;
                yield break;
            }

            if (File.Exists(pathOrUrl))
            {
                var extension = Path.GetExtension(pathOrUrl).ToLowerInvariant();

                if (extension == ".json")
                    yield return InputType.SingleFile;

                if (extension == ".tgz")
                    yield return InputType.Tgz;

                if (extension == ".zip")
                    yield return InputType.Zip;
            }

            if (Directory.Exists(pathOrUrl))
            {
                yield return InputType.Directory;
            }
        }
    }
}
