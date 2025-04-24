using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using System.IO;
using System.IO.Compression;

namespace FhirArtifactAnalyzer.Application.Services
{
    public class TgzUnzipperService
    {
        /// <summary>
        /// Extrai um arquivo .tgz, descomprimindo o conte�do .gz em um arquivo .tar tempor�rio, 
        /// e ent�o extraindo o conte�do do .tar no diret�rio de destino. O arquivo .tar � exclu�do ao final.
        /// </summary>
        /// <param name="tgzFilePath">Caminho do arquivo .tgz a ser extra�do.</param>
        /// <param name="destinationDirectory">Diret�rio onde os arquivos extra�dos ser�o salvos.</param>
        public void TgzExtractor(string tgzFilePath, string destinationDirectory)
        {
            var tempTarPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(tgzFilePath) + ".tar");

            using (var gzipFile = File.OpenRead(tgzFilePath))
            using (var tarFile = File.Create(tempTarPath))
            using (var gzipStream = new GZipStream(gzipFile, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(tarFile);
            }

            ExtractTarEntries(tempTarPath, destinationDirectory);

            File.Delete(tempTarPath);
        }

        /// <summary>
        /// Extrai todas as entradas de um arquivo .tar para o diret�rio de destino.
        /// </summary>
        /// <param name="tarFilePath">Caminho do arquivo .tar a ser extra�do.</param>
        /// <param name="destinationDirectory">Diret�rio onde os arquivos extra�dos ser�o salvos.</param>
        private void ExtractTarEntries(string tarFilePath, string destinationDirectory)
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
