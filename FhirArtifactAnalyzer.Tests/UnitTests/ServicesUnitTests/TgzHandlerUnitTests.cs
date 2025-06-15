using System.IO.Compression;
using FhirArtifactAnalyzer.Application.Services.Handlers;
using FhirArtifactAnalyzer.Domain.Constants;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesUnitTests
{
    /// <summary>
    /// Testes unitarios para o manipulador de entrada TgzHandler.
    /// Valida a capacidade de identificar, verificar tamanho e extrair arquivos .tgz corretamente.
    /// </summary>
    public class TgzHandlerUnitTests
    {
        private readonly TgzHandler _handler;

        /// <summary>
        /// Inicializa o ambiente de teste com uma instancia de TgzHandler.
        /// </summary>
        public TgzHandlerUnitTests()
        {
            _handler = new TgzHandler();
        }

        /// <summary>
        /// Garante que o metodo CanHandle retorna verdadeiro para um arquivo .tgz valido e dentro do tamanho limite.
        /// </summary>
        [Fact]
        public void CanHandle_ShouldReturnTrue_ForValidTgzFile()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Tgz);
            File.WriteAllBytes(tempFile, new byte[10]); // Arquivo pequeno, dentro do limite

            var result = _handler.CanHandle(tempFile);

            Assert.True(result);

            File.Delete(tempFile);
        }

        /// <summary>
        /// Garante que o metodo CanHandle retorna falso para um arquivo com extensao diferente de .tgz.
        /// </summary>
        [Fact]
        public void CanHandle_ShouldReturnFalse_ForNonTgzFile()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            File.WriteAllBytes(tempFile, new byte[10]);

            var result = _handler.CanHandle(tempFile);

            Assert.False(result);

            File.Delete(tempFile);
        }

        /// <summary>
        /// Garante que o metodo CanHandle retorna falso para um arquivo .tgz que excede o tamanho maximo permitido.
        /// </summary>
        [Fact]
        public void CanHandle_ShouldReturnFalse_ForOversizedFile()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Tgz);
            var oversizedData = new byte[101 * 1024 * 1024]; // 101MB, acima do limite de 100MB
            File.WriteAllBytes(tempFile, oversizedData);

            var result = _handler.CanHandle(tempFile);

            Assert.False(result);

            File.Delete(tempFile);
        }

        /// <summary>
        /// Garante que o metodo CanHandle retorna falso para um caminho inexistente.
        /// </summary>
        [Fact]
        public void CanHandle_ShouldReturnFalse_ForNonExistentFile()
        {
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + FileExtensions.Tgz);

            var result = _handler.CanHandle(nonExistentPath);

            Assert.False(result);
        }

        /// <summary>
        /// Garante que o metodo Extract cria os arquivos esperados no diretorio de destino a partir de um arquivo .tgz valido.
        /// </summary>
        [Fact]
        public void Extract_ShouldExtractFiles_ToDestinationDirectory()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var tarPath = Path.Combine(tempDir, "sample.tar");

            using (var tar = TarArchive.Create())
            {
                var filePath = Path.Combine(tempDir, "inside.txt");
                File.WriteAllText(filePath, "test content");
                tar.AddEntry("inside.txt", filePath);

                using (var stream = File.Create(tarPath))
                {
                    tar.SaveTo(stream, new WriterOptions(CompressionType.None));
                }
            }

            var tgzPath = Path.Combine(tempDir, "sample.tgz");

            using (var originalTarStream = File.OpenRead(tarPath))
            using (var compressedStream = File.Create(tgzPath))
            using (var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
            {
                originalTarStream.CopyTo(gzipStream);
            }

            var destination = Path.Combine(tempDir, "output");
            Directory.CreateDirectory(destination);

            var resultPath = _handler.Extract(tgzPath, destination);

            var extractedFile = Path.Combine(destination, "inside.txt");

            Assert.True(File.Exists(extractedFile));
            Assert.Equal(destination, resultPath);

            Directory.Delete(tempDir, true);
        }
    }
}
