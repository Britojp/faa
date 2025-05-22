using System.IO;
using Xunit;
using FhirArtifactAnalyzer.Application.Services;
using Xunit.Abstractions;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesTests
{
    public class ExtractorServiceTest
    {
        private readonly ITestOutputHelper _output;

        public ExtractorServiceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Extractor_ShouldExtractTgzFileCorrectly()
        {
            var service = new ExtractorService();
            var testFilePath = Path.Combine("Assets", "fhir_test_bundle.tgz");
            var outputDirectory = Path.Combine("TestOutput", "extracted");

            Assert.True(File.Exists(testFilePath), $"Test file '{testFilePath}' not found.");

            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, recursive: true);

            Directory.CreateDirectory(outputDirectory);

            var extractedPath = service.Extract(testFilePath, outputDirectory);

            Assert.True(Directory.Exists(extractedPath), "Output directory was not created.");

            var extractedFiles = Directory.GetFiles(extractedPath, "*", SearchOption.AllDirectories);

            foreach (var file in extractedFiles)
            {
                _output.WriteLine("Extracted file: " + file);
            }

            Assert.NotEmpty(extractedFiles);
            Assert.Contains(extractedFiles, f => f.EndsWith("Observation-example.json"));
            Assert.Contains(extractedFiles, f => f.EndsWith("Patient-example.json"));
        }
        [Fact]
        public void Extractor_ShouldExtractZipFileCorrectly()
        {
            var service = new ExtractorService();
            var testFilePath = Path.Combine("Assets", "fhir_test_bundle.zip");
            var outputDirectory = Path.Combine("TestOutput", "extracted");

            Assert.True(File.Exists(testFilePath), $"Test file '{testFilePath}' not found.");

            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, recursive: true);

            Directory.CreateDirectory(outputDirectory);

            var extractedPath = service.Extract(testFilePath, outputDirectory);

            Assert.True(Directory.Exists(extractedPath), "Output directory was not created.");

            var extractedFiles = Directory.GetFiles(extractedPath, "*", SearchOption.AllDirectories);

            foreach (var file in extractedFiles)
            {
                _output.WriteLine("Extracted file: " + file);
            }

            Assert.NotEmpty(extractedFiles);
            Assert.Contains(extractedFiles, f => f.EndsWith("Observation-example.json"));
            Assert.Contains(extractedFiles, f => f.EndsWith("Patient-example.json"));
        }
    }
}