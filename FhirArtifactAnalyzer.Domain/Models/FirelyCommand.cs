using System.Diagnostics;

namespace FhirArtifactAnalyzer.Domain.Models
{
    public class FirelyCommand
    {
        public string Command { get; init; }

        public FirelyCommand(string command)
        {
            ArgumentNullException.ThrowIfNull(command, nameof(Command));

            Command = command;
        }

        public static string Init() => new FirelyCommand("init --version r4").Run();

        public static string Clear() => new FirelyCommand("clear").Run();
        
        public static string Push(string fileName) => new FirelyCommand($"push {fileName}").Run();
        
        public static string Validate() => new FirelyCommand("validate --push").Run();
        
        public static string Save(string outcomePath) => new FirelyCommand($"save {outcomePath}").Run();
        
        public static string Drop() => new FirelyCommand("drop").Run();

        public string Run()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Options.WorkingDirectory, nameof(Options.WorkingDirectory));

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fhir",
                    Arguments = Command,
                    WorkingDirectory = Options.WorkingDirectory,
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

        public static class Options
        {
            public static string WorkingDirectory { get; set; } = default!;
        }
    }
}
