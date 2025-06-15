using FhirArtifactAnalyzer.Application.Services.Handlers;
using System.IO.Compression;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesUnitTests
{
    /// <summary>
    /// Testes unitarios para a classe ZipHandler.
    /// Valida os cenarios de deteccao e extracao de arquivos no formato ZIP.
    /// </summary>
    public class ZipHandlerUnitTests
    {
        /// <summary>
        /// Deve retornar false quando o arquivo nao existir.
        /// </summary>
        [Fact]
        public void CanHandle_DeveRetornarFalse_QuandoArquivoNaoExistir()
        {
            var handler = new ZipHandler();
            var caminhoInexistente = Path.Combine(Path.GetTempPath(), "arquivo_inexistente.zip");

            var resultado = handler.CanHandle(caminhoInexistente);

            Assert.False(resultado);
        }

        /// <summary>
        /// Deve retornar false quando o arquivo nao for um ZIP.
        /// </summary>
        [Fact]
        public void CanHandle_DeveRetornarFalse_QuandoNaoForZip()
        {
            var handler = new ZipHandler();
            var arquivoTemporario = Path.GetTempFileName();

            var resultado = handler.CanHandle(arquivoTemporario);

            Assert.False(resultado);

            File.Delete(arquivoTemporario);
        }

        /// <summary>
        /// Deve retornar false quando o arquivo ZIP exceder o tamanho maximo permitido.
        /// </summary>
        [Fact]
        public void CanHandle_DeveRetornarFalse_QuandoArquivoExcederTamanhoMaximo()
        {
            var handler = new ZipHandler();
            var caminhoZip = Path.Combine(Path.GetTempPath(), "grande.zip");

            using (var stream = new FileStream(caminhoZip, FileMode.Create))
            {
                stream.SetLength(150 * 1024 * 1024);
            }

            var resultado = handler.CanHandle(caminhoZip);

            Assert.False(resultado);

            File.Delete(caminhoZip);
        }

        /// <summary>
        /// Deve retornar true quando o arquivo for um ZIP valido e dentro do limite de tamanho.
        /// </summary>
        [Fact]
        public void CanHandle_DeveRetornarTrue_QuandoZipValidoDentroDoLimite()
        {
            var handler = new ZipHandler();
            var caminhoZip = Path.Combine(Path.GetTempPath(), "valido.zip");

            using (var arquivo = ZipFile.Open(caminhoZip, ZipArchiveMode.Create))
            {
                var entrada = arquivo.CreateEntry("teste.txt");
                using var writer = new StreamWriter(entrada.Open());
                writer.Write("conteudo de teste");
            }

            var resultado = handler.CanHandle(caminhoZip);

            Assert.True(resultado);

            File.Delete(caminhoZip);
        }

        /// <summary>
        /// Deve extrair corretamente o conteudo de um arquivo ZIP para o diretorio de destino.
        /// </summary>
        [Fact]
        public void Extract_DeveExtrairConteudoZipCorretamente()
        {
            var handler = new ZipHandler();
            var pastaTemporaria = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(pastaTemporaria);

            var caminhoZip = Path.Combine(pastaTemporaria, "teste.zip");
            var diretorioSaida = Path.Combine(pastaTemporaria, "saida");

            using (var arquivo = ZipFile.Open(caminhoZip, ZipArchiveMode.Create))
            {
                var entrada = arquivo.CreateEntry("arquivo.txt");
                using var writer = new StreamWriter(entrada.Open());
                writer.Write("conteudo de teste");
            }

            var resultado = handler.Extract(caminhoZip, diretorioSaida);

            Assert.True(Directory.Exists(resultado));

            var caminhoArquivoExtraido = Path.Combine(diretorioSaida, "arquivo.txt");
            Assert.True(File.Exists(caminhoArquivoExtraido));

            var conteudo = File.ReadAllText(caminhoArquivoExtraido);
            Assert.Equal("conteudo de teste", conteudo);

            Directory.Delete(pastaTemporaria, recursive: true);
        }
    }
}
