using System.Diagnostics;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Settings;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;

namespace FhirArtifactAnalyzer.Domain.Validation
{
    public class LocalFhirValidatorService : IFhirValidatorService
    {
        private readonly string _workingDirectory;
        private readonly IFhirParserFactory _parseFactory;

        public LocalFhirValidatorService(IOptions<ArtifactValidationSettings> settings, IFhirParserFactory parserFactory)
        {   
            _workingDirectory = settings.Value.WorkingDirectory ?? throw new ArgumentNullException("WorkingDirectory");

            _parseFactory = parserFactory;

            var manifestPath = Path.Combine(_workingDirectory, "package.json");

            if (!Directory.Exists(_workingDirectory))
            {
                throw new DirectoryNotFoundException($"Directory {_workingDirectory} not found");
            }

            if (!File.Exists(manifestPath))
            {
                RunFirelyCommand("init --version r4");
            }
        }

        public OperationOutcome Validate(string fileName)
        {
            string resourcePath = Path.Combine(_workingDirectory, fileName);

            RunFirelyCommand("clear");
            RunFirelyCommand($"push {fileName}");
            RunFirelyCommand("validate --push");
        
            string outcomePath = Path.Combine(_workingDirectory, "outcome.json");
            RunFirelyCommand($"save {outcomePath}");
        
            RunFirelyCommand("drop");

            var jsonOutcome = File.ReadAllText(outcomePath);
            var parser = _parseFactory.GetParserForFile("outcome.json");
            return parser.Parse<OperationOutcome>(jsonOutcome);
        }
        
        private string RunFirelyCommand(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fhir",
                    Arguments = arguments,
                    WorkingDirectory = _workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(stderr))
            {
                throw new Exception($"Firely CLI error: {stderr}");
            }

            return stdout;
        }
    }
}