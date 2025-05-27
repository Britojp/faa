using Spectre.Console;
using Spectre.Console.Cli;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Cli
{
    /// <summary>
    /// Responsavel pela entrada CLI, dando feedback pelo CMD de acordo com os artefatos inseridos.
    /// </summary>
    public class AnalyzeCommand : AsyncCommand<AnalyzeSettings>
    {
        private readonly IArtifactProcessor _processor;
        private readonly IInputIdentifier _identifier;

        public AnalyzeCommand(IArtifactProcessor processor, IInputIdentifier identifier)
        {
            _processor = processor;
            _identifier = identifier;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, AnalyzeSettings settings)
        {
            var inputType = _identifier.GetInputType(settings.Path);

            if (inputType is null)
            {
                AnsiConsole.MarkupLine("[red]Não foi possível determinar o tipo da entrada.[/]");
                return -1;
            }

            var input = new InputSource
            {
                PathOrUrl = settings.Path,
                RegexFilter = settings.Regex,
                Type = inputType.Value
            };

            var result = await _processor.ProcessInputAsync(input);

            foreach (var artifact in result)
            {
                AnsiConsole.MarkupLine($"[blue]{artifact.FilePath}[/] => [green]{artifact.ResourceType}[/] | [bold]{(artifact.IsRelevantFhirResource ? "RELEVANTE" : "IGNORADO")}[/]");
                if (!artifact.IsRelevantFhirResource)
                    AnsiConsole.MarkupLine($"[red]Motivo:[/] {artifact.ReasonIgnored}");
            }

            return 0;
        }
    }
}
