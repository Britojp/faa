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
        /// Extrai um arquivo .tgz, descomprimindo o conteúdo .gz em um arquivo .tar temporário, 
        /// e então extraindo o conteúdo do .tar no diretório de destino. O arquivo .tar é excluído ao final.
        /// </summary>
        /// <param name="tgzFilePath">Caminho do arquivo .tgz a ser extraído.</param>
        /// <param name="destinationDirectory">Diretório onde os arquivos extraídos serão salvos.</param>
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
        /// Extrai todas as entradas de um arquivo .tar para o diretório de destino.
        /// </summary>
        /// <param name="tarFilePath">Caminho do arquivo .tar a ser extraído.</param>
        /// <param name="destinationDirectory">Diretório onde os arquivos extraídos serão salvos.</param>
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
