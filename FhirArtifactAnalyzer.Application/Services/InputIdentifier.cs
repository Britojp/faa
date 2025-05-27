using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Constants;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Application.Services
{
    /// <summary>
    /// Responsavel pela identificação do tipo de entrada.
    /// </summary>
    public class InputIdentifier : IInputIdentifier
    {
        public InputType? GetInputType(string pathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(pathOrUrl))
                return null;

            if (Uri.IsWellFormedUriString(pathOrUrl, UriKind.Absolute))
                return InputType.Url;

            if (File.Exists(pathOrUrl))
            {
                var extension = Path.GetExtension(pathOrUrl).ToLowerInvariant();

                return extension switch
                {
                    FileExtensions.Json => InputType.SingleFile,
                    FileExtensions.Tgz => InputType.Tgz,
                    FileExtensions.Zip => InputType.Zip,
                    _ => null
                };
            }

            if (Directory.Exists(pathOrUrl)) return InputType.Directory;

            return null;
        }
    }
}
