using System.Diagnostics;
using FhirArtifactAnalyzer.Domain.Abstractions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirArtifactAnalyzer.Infrastructure.Services
{
    public class LocalFhirValidatorService : IFhirValidatorService<Resource>
    {
        private readonly string _workingDirectory;

        public LocalFhirValidatorService(string workDirectory)
        {   
            if (!Directory.Exists(workDirectory))
            {
                throw new DirectoryNotFoundException($"Directory {workDirectory} not found");
            }

            _workingDirectory = workDirectory;
            var manifestPath = Path.Combine(_workingDirectory, "package.json");

            if (!File.Exists(manifestPath))
            {
                RunFirelyCommand("init --version r4");
            }
        }

        public OperationOutcome Validate(Resource resource)
        {
            string resourcePath = Path.Combine(_workingDirectory, "resource.json");

            var json = new FhirJsonSerializer().SerializeToString(resource);
            File.WriteAllText(resourcePath, json);

            RunFirelyCommand("fhir clear");
            RunFirelyCommand($"push resource.json");
            RunFirelyCommand("validate --push");
        
            string outcomePath = Path.Combine(_workingDirectory, "validation-outcome.json");
            RunFirelyCommand($"save {outcomePath}");
        
            var jsonOutcome = File.ReadAllText(outcomePath);
            var parser = new FhirJsonParser();
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