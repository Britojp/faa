using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Domain.Settings;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;

namespace FhirArtifactAnalyzer.Domain.Validation
{
    public class LocalFhirValidator : IFhirValidator
    {
        private readonly IFhirParserFactory _parserFactory;
        private readonly string _workingDirectory;

        public LocalFhirValidator(IFhirParserFactory parserFactory, IOptions<ArtifactValidationSettings> settings)
        {   
            _parserFactory = parserFactory;
            _workingDirectory = settings.Value.WorkingDirectory ?? throw new ArgumentNullException("WorkingDirectory");

            if (!Directory.Exists(_workingDirectory))
            {
                throw new DirectoryNotFoundException($"Directory {_workingDirectory} not found");
            }

            var manifestPath = Path.Combine(_workingDirectory, "package.json");
            
            if (!File.Exists(manifestPath))
            {
                FirelyCommand.Options.WorkingDirectory = _workingDirectory;
                FirelyCommand.Init();
            }
        }

        public OperationOutcome Validate(string fileName)
        {
            FirelyCommand.Clear();
            FirelyCommand.Push(fileName);
            FirelyCommand.Validate();
        
            var outcomePath = Path.Combine(_workingDirectory, "outcome.json");
            FirelyCommand.Save(outcomePath);
            FirelyCommand.Drop();

            var jsonOutcome = File.ReadAllText(outcomePath);
            var parser = _parserFactory.GetParserForFile("outcome.json");
            
            return parser.Parse<OperationOutcome>(jsonOutcome);
        }
    }
}