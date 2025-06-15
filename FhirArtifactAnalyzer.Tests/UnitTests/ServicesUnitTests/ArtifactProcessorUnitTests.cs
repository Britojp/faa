using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using Moq;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesUnitTests
{
    /// <summary>
    /// Testes unitarios para o serviço ArtifactProcessor.
    /// Garante o correto processamento de diferentes tipos de entrada,
    /// validando a aplicacao de filtros e a integracao com os componentes dependentes.
    /// </summary>
    public class ArtifactProcessorUnitTests
    {
        private readonly Mock<IDirectoryNavigator> _navigatorMock;
        private readonly Mock<IExtractorService> _extractorMock;
        private readonly Mock<IJsonArtifactAnalyzer> _analyzerMock;
        private readonly ArtifactProcessor _processor;

        /// <summary>
        /// Inicializa o ambiente de teste com dependencias simuladas.
        /// </summary>
        public ArtifactProcessorUnitTests()
        {
            _navigatorMock = new Mock<IDirectoryNavigator>();
            _extractorMock = new Mock<IExtractorService>();
            _analyzerMock = new Mock<IJsonArtifactAnalyzer>();

            _processor = new ArtifactProcessor(
                _navigatorMock.Object,
                _extractorMock.Object,
                _analyzerMock.Object);
        }

        /// <summary>
        /// Metodo auxiliar para simular o retorno da analise de artefatos.
        /// </summary>
        private Task<FhirArtifactInfo> FakeAnalyzeAsync(string path, InputSource source)
        {
            return Task.FromResult(new FhirArtifactInfo { FilePath = path });
        }

        
        /// <summary>
        /// Garante que o processamento de um unico arquivo individual
        /// retorna o artefato esperado.
        /// </summary>
        [Fact]
        public async Task ProcessInputAsync_ShouldProcessSingleFileCorrectly()
        {
            var filePath = @"C:\file.json";

            _analyzerMock
                .Setup(a => a.AnalyzeAsync(It.IsAny<string>(), It.IsAny<InputSource>()))
                .Returns((string path, InputSource source) => FakeAnalyzeAsync(path, source));

            var source = new InputSource
            {
                Type = InputType.SingleFile,
                PathOrUrl = filePath
            };

            var result = (await _processor.ProcessInputAsync(source)).ToList();

            Assert.Single(result);
            Assert.Equal(filePath, result[0].FilePath);
        }

        /// <summary>
        /// Garante que o processamento de um arquivo compactado .tgz realiza a extracao,
        /// coleta os arquivos extraidos e executa a analise corretamente.
        /// </summary>
        [Fact]
        public async Task ProcessInputAsync_ShouldProcessTgzFileCorrectly()
        {
            var extractedDir = @"C:\temp\extracted";
            var extractedFiles = new[] { Path.Combine(extractedDir, "file1.json"), Path.Combine(extractedDir, "file2.json") };

            _extractorMock
                .Setup(e => e.Extract(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(extractedDir);

            Directory.CreateDirectory(extractedDir);
            foreach (var file in extractedFiles)
                File.Create(file).Dispose();

            _analyzerMock
                .Setup(a => a.AnalyzeAsync(It.IsAny<string>(), It.IsAny<InputSource>()))
                .Returns((string path, InputSource source) => FakeAnalyzeAsync(path, source));

            var source = new InputSource
            {
                Type = InputType.Tgz,
                PathOrUrl = @"C:\input\file.tgz"
            };

            var result = (await _processor.ProcessInputAsync(source)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.FilePath == extractedFiles[0]);
            Assert.Contains(result, r => r.FilePath == extractedFiles[1]);

            Directory.Delete(extractedDir, true);
        }

        /// <summary>
        /// Garante que o processamento de um arquivo compactado .zip realiza a extracao,
        /// coleta os arquivos extraidos e executa a analise corretamente.
        /// </summary>
        [Fact]
        public async Task ProcessInputAsync_ShouldProcessZipFileCorrectly()
        {
            var extractedDir = @"C:\temp\extracted";
            var extractedFiles = new[] { Path.Combine(extractedDir, "file1.json"), Path.Combine(extractedDir, "file2.json") };

            _extractorMock
                .Setup(e => e.Extract(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(extractedDir);

            Directory.CreateDirectory(extractedDir);
            foreach (var file in extractedFiles)
                File.Create(file).Dispose();

            _analyzerMock
                .Setup(a => a.AnalyzeAsync(It.IsAny<string>(), It.IsAny<InputSource>()))
                .Returns((string path, InputSource source) => FakeAnalyzeAsync(path, source));

            var source = new InputSource
            {
                Type = InputType.Zip,
                PathOrUrl = @"C:\input\file.zip"
            };

            var result = (await _processor.ProcessInputAsync(source)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.FilePath == extractedFiles[0]);
            Assert.Contains(result, r => r.FilePath == extractedFiles[1]);

            Directory.Delete(extractedDir, true);
        }

        

        /// <summary>
        /// Garante que ao receber um tipo de entrada URL, o metodo lanca a excecao NotImplementedException.
        /// </summary>
        [Fact]
        public async Task ProcessInputAsync_ShouldThrowForUrlType()
        {
            var source = new InputSource
            {
                Type = InputType.Url,
                PathOrUrl = "http://example.com"
            };

            await Assert.ThrowsAsync<NotImplementedException>(() => _processor.ProcessInputAsync(source));
        }
    }
}
