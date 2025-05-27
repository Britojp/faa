using Spectre.Console.Cli;
using FhirArtifactAnalyzer.Domain.Models;


namespace FhirArtifactAnalyzer.Cli
{
    /// <summary>
    /// Responsável pelas configurações do AnalyzeCommand(CLI).
    /// </summary>
    public class AnalyzeSettings : CommandSettings
    {
        [CommandArgument(0, "<path>")]
        public string Path { get; set; } = string.Empty;

        [CommandOption("-r|--regex <REGEX>")]
        public string Regex { get; set; } = string.Empty;
    }

}
