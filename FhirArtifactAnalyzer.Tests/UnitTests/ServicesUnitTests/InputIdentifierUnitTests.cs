using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesUnitTests
{
    /// <summary>
    /// Testes unitarios para o componente responsavel pela identificacao do tipo de entrada.
    /// Garante que entradas como URLs, arquivos e diretorios sejam corretamente reconhecidos.
    /// </summary>
    public class InputIdentifierUnitTests
    {
        private readonly InputIdentifier _sut;

        /// <summary>
        /// Inicializa a classe de testes com a instancia do componente de identificacao.
        /// </summary>
        public InputIdentifierUnitTests()
        {
            _sut = new InputIdentifier();
        }

        /// <summary>
        /// Deve retornar nulo quando o caminho fornecido for nulo.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnNull_WhenPathIsNull()
        {
            string? input = null;

            var result = _sut.GetInputType(input);

            Assert.Null(result);
        }

        /// <summary>
        /// Deve retornar nulo quando o caminho fornecido for uma string vazia.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnNull_WhenPathIsEmpty()
        {
            var input = string.Empty;

            var result = _sut.GetInputType(input);

            Assert.Null(result);
        }

        /// <summary>
        /// Deve retornar nulo quando o caminho fornecido conter apenas espacos.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnNull_WhenPathIsWhitespace()
        {
            var input = "   ";

            var result = _sut.GetInputType(input);

            Assert.Null(result);
        }

        /// <summary>
        /// Deve identificar corretamente uma URL valida como entrada do tipo Url.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnUrl_WhenInputIsValidAbsoluteUrl()
        {
            var input = "https://example.com/fhir.json";

            var result = _sut.GetInputType(input);

            Assert.Equal(InputType.Url, result);
        }

        /// <summary>
        /// Deve identificar corretamente um arquivo com extensao json como entrada do tipo SingleFile.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnSingleFile_WhenFileHasJsonExtension()
        {
            var tempFile = Path.GetTempFileName();
            File.Move(tempFile, tempFile + ".json");
            tempFile += ".json";

            try
            {
                var result = _sut.GetInputType(tempFile);

                Assert.Equal(InputType.SingleFile, result);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        /// <summary>
        /// Deve identificar corretamente um arquivo com extensao tgz como entrada do tipo Tgz.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnTgz_WhenFileHasTgzExtension()
        {
            var tempPath = Path.GetTempFileName();
            File.Move(tempPath, tempPath + ".tgz");
            tempPath += ".tgz";

            try
            {
                var result = _sut.GetInputType(tempPath);

                Assert.Equal(InputType.Tgz, result);
            }
            finally
            {
                File.Delete(tempPath);
            }
        }

        /// <summary>
        /// Deve identificar corretamente um arquivo com extensao zip como entrada do tipo Zip.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnZip_WhenFileHasZipExtension()
        {
            var tempPath = Path.GetTempFileName();
            File.Move(tempPath, tempPath + ".zip");
            tempPath += ".zip";

            try
            {
                var result = _sut.GetInputType(tempPath);

                Assert.Equal(InputType.Zip, result);
            }
            finally
            {
                File.Delete(tempPath);
            }
        }

        /// <summary>
        /// Deve retornar nulo quando o arquivo tiver uma extensao desconhecida.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnNull_WhenFileHasUnknownExtension()
        {
            var tempPath = Path.GetTempFileName();
            File.Move(tempPath, tempPath + ".xyz");
            tempPath += ".xyz";

            try
            {
                var result = _sut.GetInputType(tempPath);

                Assert.Null(result);
            }
            finally
            {
                File.Delete(tempPath);
            }
        }

        /// <summary>
        /// Deve identificar corretamente um diretorio existente como entrada do tipo Directory.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnDirectory_WhenInputIsExistingDirectory()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var result = _sut.GetInputType(tempDir);

                Assert.Equal(InputType.Directory, result);
            }
            finally
            {
                Directory.Delete(tempDir);
            }
        }

        /// <summary>
        /// Deve retornar nulo quando o caminho nao representar um arquivo, diretorio ou URL valido.
        /// </summary>
        [Fact]
        public void GetInputType_ShouldReturnNull_WhenPathIsInvalid()
        {
            var invalidPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            var result = _sut.GetInputType(invalidPath);

            Assert.Null(result);
        }
    }
}
