using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Constants;
using System.IO.Compression;

namespace FhirArtifactAnalyzer.Application.Services.Handlers
{
    /// <summary>
    /// Responsável por extrair arquivos no formato .zip para um diretório especificado.
    /// </summary>
    public class ZipHandler : IInputHandler
    {
        private const long _maxSize = 100 * 1024 * 1024;

        public bool CanHandle(string path)
        {
            if (!File.Exists(path)) return false;

            var isZip = Path.GetExtension(path).Equals(FileExtensions.Zip, StringComparison.OrdinalIgnoreCase);
            if (!isZip) return false;

            var fileInfo = new FileInfo(path);
            return fileInfo.Length <= _maxSize;
        }

        /// <summary>
        /// Extrai um arquivo .zip para o diretório de destino, sobrescrevendo arquivos existentes se necessário.
        /// </summary>
        public string Extract(string path, string destinationDirectory)
        {
            ZipFile.ExtractToDirectory(path, destinationDirectory, true);
            return destinationDirectory;
        }
    }
}