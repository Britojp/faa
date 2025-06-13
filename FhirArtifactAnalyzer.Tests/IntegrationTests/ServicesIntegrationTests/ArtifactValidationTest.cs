using FhirArtifactAnalyzer.Application.Services;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Utils;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Moq;
using System.Text;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesTests
{
    public class ArtifactValidationTest
    {
    
        [Fact]
        public void Execute_ReturnsExpectedOutcome()
        {
            var solutionDir = SolutionPathFinder.GetSolutionDirectory();
            var workingDir = Path.Combine(solutionDir, "FhirArtifactAnalyzer.ValidationArtifacts");

            var fhirJsonParser = new FhirJsonParser();
            var fhirJsonSerializer = new FhirJsonSerializer();

            var resourceJson = File.ReadAllText(Path.Combine(workingDir, "mulher.json"));
            var expectedOutcomeJson = File.ReadAllText(Path.Combine(workingDir, "outcome_true_expected.json"));

            var mockValidator = new Mock<IFhirValidator>();

            var resource = fhirJsonParser.Parse<Resource>(resourceJson);
            var expectedOutcome = fhirJsonParser.Parse<OperationOutcome>(expectedOutcomeJson);

            mockValidator.Setup(v => v.Validate("mulher.json")).Returns(expectedOutcome);

            var sut = new ArtifactValidation(mockValidator.Object);

            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(resourceJson));

            var actualOutcome = sut.Execute("mulher.json");

            var actualJson = fhirJsonSerializer.SerializeToString(actualOutcome);
            var expectedJson = fhirJsonSerializer.SerializeToString(expectedOutcome);

            Assert.Equal(expectedJson, actualJson);            
        }

        [Fact]
        public void Execute_ReturnsExpectedIssues()
        {
            var solutionDir = SolutionPathFinder.GetSolutionDirectory();
            var workingDir = Path.Combine(solutionDir, "FhirArtifactAnalyzer.ValidationArtifacts");

            var fhirJsonParser = new FhirJsonParser();
            var fhirJsonSerializer = new FhirJsonSerializer();

            var resourceJson = File.ReadAllText(Path.Combine(workingDir, "homem.json"));
            var expectedOutcomeJson = File.ReadAllText(Path.Combine(workingDir, "outcome_fail_expected.json"));

            var mockValidator = new Mock<IFhirValidator>();

            var resource = fhirJsonParser.Parse<Resource>(resourceJson);
            var expectedOutcome = fhirJsonParser.Parse<OperationOutcome>(expectedOutcomeJson);

            mockValidator.Setup(v => v.Validate("homem.json")).Returns(expectedOutcome);

            var sut = new ArtifactValidation(mockValidator.Object);

            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(resourceJson));

            var actualOutcome = sut.Execute("homem.json");

            var actualJson = fhirJsonSerializer.SerializeToString(actualOutcome);
            var expectedJson = fhirJsonSerializer.SerializeToString(expectedOutcome);

            Assert.Equal(expectedJson, actualJson);           
        }
    }
}