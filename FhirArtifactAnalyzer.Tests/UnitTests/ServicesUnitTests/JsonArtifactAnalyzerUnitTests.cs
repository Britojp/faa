using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Domain.Abstractions;
using Hl7.Fhir.Model;
using Moq;
using Task = System.Threading.Tasks.Task;
using System.Text.Json;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesUnitTests
{
    public class JsonArtifactAnalyzerUnitTests
    {
        private readonly Mock<IFhirParserFactory> _parserFactoryMock;
        private readonly Mock<IFhirParser> _parserMock;
        private readonly JsonArtifactAnalyzer _analyzer;

        public JsonArtifactAnalyzerUnitTests()
        {
            _parserFactoryMock = new Mock<IFhirParserFactory>();
            _parserMock = new Mock<IFhirParser>();
            _analyzer = new JsonArtifactAnalyzer(_parserFactoryMock.Object);
        }

        /// <summary>
        /// Deve retornar artefato ignorado quando o arquivo nao existir
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_ReturnsIgnored_WhenFileDoesNotExist()
        {
            var path = "nonexistent.json";
            var source = new InputSource { PathOrUrl = path };

            var result = await _analyzer.AnalyzeAsync(path, source);

            Assert.False(result.IsRelevantFhirResource);
            Assert.Equal("Arquivo nao encontrado.", result.ReasonIgnored);
        }

        /// <summary>
        /// Deve retornar artefato ignorado quando a extensao nao for .json
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_ReturnsIgnored_WhenExtensionIsNotJson()
        {
            var path = "test.txt";
            File.WriteAllText(path, "{}");
            var source = new InputSource { PathOrUrl = path };

            var result = await _analyzer.AnalyzeAsync(path, source);

            Assert.False(result.IsRelevantFhirResource);
            Assert.Equal("Extensao nao e .json.", result.ReasonIgnored);

            File.Delete(path);
        }

        /// <summary>
        /// Deve retornar artefato ignorado quando o arquivo for maior que 10MB
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_ReturnsIgnored_WhenFileTooLarge()
        {
            var path = "large.json";
            var content = new string('a', 10 * 1024 * 1024 + 1);
            File.WriteAllText(path, content);
            var source = new InputSource { PathOrUrl = path };

            var result = await _analyzer.AnalyzeAsync(path, source);

            Assert.False(result.IsRelevantFhirResource);
            Assert.Equal("Arquivo excede o tamanho maximo permitido.", result.ReasonIgnored);

            File.Delete(path);
        }

        /// <summary>
        /// Deve retornar artefato valido quando o recurso for relevante
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_ReturnsValidArtifact_WhenResourceIsRelevant()
        {
            var path = "valid.json";
            var content = "{}";
            File.WriteAllText(path, content);

            var resource = new StructureDefinition();
            _parserMock.Setup(p => p.Parse<Resource>(It.IsAny<string>())).Returns(resource);
            _parserFactoryMock.Setup(f => f.GetParserForFile(It.IsAny<string>())).Returns(_parserMock.Object);

            var source = new InputSource { PathOrUrl = path };

            var result = await _analyzer.AnalyzeAsync(path, source);

            Assert.True(result.IsRelevantFhirResource);
            Assert.Equal("StructureDefinition", result.ResourceType);
            Assert.Null(result.ReasonIgnored);

            File.Delete(path);
        }

        /// <summary>
        /// Deve retornar artefato ignorado quando o recurso FHIR nao for relevante
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_ReturnsIgnored_WhenResourceIsIrrelevant()
        {
            var path = "irrelevant.json";
            File.WriteAllText(path, "{}");

            var resource = new Patient();
            _parserMock.Setup(p => p.Parse<Resource>(It.IsAny<string>())).Returns(resource);
            _parserFactoryMock.Setup(f => f.GetParserForFile(It.IsAny<string>())).Returns(_parserMock.Object);

            var source = new InputSource { PathOrUrl = path };

            var result = await _analyzer.AnalyzeAsync(path, source);

            Assert.False(result.IsRelevantFhirResource);
            Assert.Equal("resourceType 'Patient' nao e relevante.", result.ReasonIgnored);

            File.Delete(path);
        }

        /// <summary>
        /// Deve retornar ignorado quando o conteúdo do JSON é inválido
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_ReturnsIgnored_WhenJsonIsMalformed()
        {
            var path = "malformed.json";
            File.WriteAllText(path, "{ invalid json ");

            _parserMock.Setup(p => p.Parse<Resource>(It.IsAny<string>())).Throws(new Exception("Erro de parse"));
            _parserFactoryMock.Setup(f => f.GetParserForFile(It.IsAny<string>())).Returns(_parserMock.Object);

            var source = new InputSource { PathOrUrl = path };

            var result = await _analyzer.AnalyzeAsync(path, source);

            Assert.False(result.IsRelevantFhirResource);
            Assert.False(result.IsWellFormedJson);
            Assert.StartsWith("Erro ao analisar JSON:", result.ReasonIgnored);

            File.Delete(path);
        }

        /// <summary>
        /// Verifica se um arquivo JSON malformado
        /// eh identificado corretamente como ignorado
        /// e marcado como nao bem formado.
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_RetornaIgnorado_QuandoJsonEhMalformado()
        {
            var caminho = "malformed.json";
            File.WriteAllText(caminho, "{ invalid json }");

            _parserFactoryMock.Setup(f => f.GetParserForFile(It.IsAny<string>())).Returns(_parserMock.Object);
            _parserMock.Setup(p => p.Parse<Resource>(It.IsAny<string>())).Throws(new JsonException("Formato invalido"));

            var origem = new InputSource { PathOrUrl = caminho };

            var resultado = await _analyzer.AnalyzeAsync(caminho, origem);

            Assert.False(resultado.IsRelevantFhirResource);
            Assert.False(resultado.IsWellFormedJson);
            Assert.Contains("Erro ao analisar JSON", resultado.ReasonIgnored);

            File.Delete(caminho);
        }

        /// <summary>
        /// Verifica se o analisador ignora o arquivo corretamente
        /// quando ocorre uma excecao de permissao negada.
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_RetornaIgnorado_QuandoPermissaoNegada()
        {
            var caminho = "denied.json";
            File.WriteAllText(caminho, "{}");

            _parserFactoryMock.Setup(f => f.GetParserForFile(It.IsAny<string>())).Throws<UnauthorizedAccessException>();

            var origem = new InputSource { PathOrUrl = caminho };

            var resultado = await _analyzer.AnalyzeAsync(caminho, origem);

            Assert.False(resultado.IsRelevantFhirResource);
            Assert.Equal("Permissao negada ao acessar o arquivo.", resultado.ReasonIgnored);

            File.Delete(caminho);
        }

        /// <summary>
        /// Verifica se o analisador ignora o arquivo corretamente
        /// quando ocorre uma excecao de leitura do tipo IOException.
        /// </summary>
        [Fact]
        public async Task AnalyzeAsync_RetornaIgnorado_QuandoIOExceptionEhLancada()
        {
            var caminho = "ioerror.json";
            File.WriteAllText(caminho, "{}");

            _parserFactoryMock.Setup(f => f.GetParserForFile(It.IsAny<string>())).Throws(new IOException("Erro de leitura"));

            var origem = new InputSource { PathOrUrl = caminho };

            var resultado = await _analyzer.AnalyzeAsync(caminho, origem);

            Assert.False(resultado.IsRelevantFhirResource);
            Assert.Contains("Erro ao ler o arquivo", resultado.ReasonIgnored);

            File.Delete(caminho);
        }
    }
}
