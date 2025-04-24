using System.IO;
using Xunit;
using FhirArtifactAnalyzer.Application.Services;
using Xunit.Abstractions;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesTests
{
    public class TgzUnzipperServiceTest
    {
        private readonly ITestOutputHelper _output;

        public TgzUnzipperServiceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TgzUnzipperService_ExtrairTgz_DeveExtrairCorretamente()
        {
            // Arrange
            var service = new TgzUnzipperService();
            string tgzArquivoTeste = Path.Combine("Assets", "fhir_test_bundle.tgz");
            string diretorioDestino = Path.Combine("TestOutput", "extracao");

            Assert.True(File.Exists(tgzArquivoTeste), $"Arquivo de teste '{tgzArquivoTeste}' não encontrado.");

            if (Directory.Exists(diretorioDestino))
                Directory.Delete(diretorioDestino, true);

            Directory.CreateDirectory(diretorioDestino);

            // Act
            service.ExtrairTgz(tgzArquivoTeste, diretorioDestino);

            // Assert
            Assert.True(Directory.Exists(diretorioDestino), "Diretório de destino não foi criado.");

            var arquivos = Directory.GetFiles(diretorioDestino, "*", SearchOption.AllDirectories);

            foreach (var arquivo in arquivos)
            {
                _output.WriteLine("Arquivo extraído: " + arquivo);
            }

            Assert.NotEmpty(arquivos); 

            Assert.Contains(arquivos, f => f.EndsWith("Observation-example.json"));
            Assert.Contains(arquivos, f => f.EndsWith("Patient-example.json"));
        }

    }
}
