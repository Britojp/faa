using FhirArtifactAnalyzer.Domain.Abstractions;
using SharpCompress.Common;
using System.IO.Compression;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using Hl7.Fhir.Model;
using FhirArtifactAnalyzer.Domain.Constants;

namespace FhirArtifactAnalyzer.Application.Services.Handlers
{
    /// <summary>
    /// Responsável por extrair arquivos no formato .tgz, que são compostos por um arquivo .tar compactado com gzip (.gz).
    /// </summary>
    public class TgzHandler : IInputHandler
    {
        private const long _maxSize = 100 * 1024 * 1024;

        public bool CanHandle(string path)
        {
            if (!File.Exists(path)) return false;

            var isTgz = Path.GetExtension(path).Equals(FileExtensions.Tgz, StringComparison.OrdinalIgnoreCase);
            if (!isTgz) return false;

            var fileInfo = new FileInfo(path);
            return fileInfo.Length <= _maxSize;
        }

        /// <summary>
        /// Extrai um arquivo .tgz, descomprimindo o conteúdo .gz em um arquivo .tar temporário, 
        /// e então extraindo o conteúdo do .tar no diretório de destino. O arquivo .tar é excluído ao final.
        /// </summary>
        public string Extract(string path, string destinationDirectory)
        {
            var tempPath = Path.GetTempPath();
            var tarArchive = Path.GetFileNameWithoutExtension(path) + FileExtensions.Tar;
            var tempTarPath = Path.Combine(tempPath, tarArchive);

            using (var gzipFile = File.OpenRead(path))
            using (var gzipStream = new GZipStream(gzipFile, CompressionMode.Decompress))
            using (var tarFile = File.Create(tempTarPath))
            {
                gzipStream.CopyTo(tarFile);
            }

            ExtractTar(tempTarPath, destinationDirectory);

            File.Delete(tempTarPath);

            return destinationDirectory;
        }

        /// <summary>
        /// Extrai todas as entradas de um arquivo .tar para o diretório de destino.
        /// </summary>
        private static void ExtractTar(string tarFilePath, string destinationDirectory)
        {
            using var tar = TarArchive.Open(tarFilePath);
            foreach (var entry in tar.Entries)
            {
                if (entry.IsDirectory) continue;

                entry.WriteToDirectory(destinationDirectory, new ExtractionOptions
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
            }
        }
    }
}