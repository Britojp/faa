using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using System.IO;
using System.IO.Compression;

namespace FhirArtifactAnalyzer.Application.Services
{
    public class TgzUnzipperService
    {
        // ------------ Descomprime o .gz de um arquivo .tgz para .tar, extrai esse .tar e limpa o arquivo temporário ----------------
        public void ExtrairTgz(string tgzCaminhoArquivo, string diretorioDestino)
        {
            // cria um caminho temporario para o .tar ser salvo após descomprimir o .gz
            string caminhoTar = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(tgzCaminhoArquivo) + ".tar");

            // 1. descomprimir o .gz para .tar
            using (var arquivoGzip = File.OpenRead(tgzCaminhoArquivo))
            using (var arquivoTar = File.Create(caminhoTar))
            using (var gzipStream = new GZipStream(arquivoGzip, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(arquivoTar);
            }

            // 2. extrair o .tar
            using (var tar = TarArchive.Open(caminhoTar))
            {
                foreach (var entrada in tar.Entries)
                {
                    if (!entrada.IsDirectory)
                    {
                        entrada.WriteToDirectory(diretorioDestino, new ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }

            // excluir o arquivo .tar temporário
            File.Delete(caminhoTar);
        }
    }
}
