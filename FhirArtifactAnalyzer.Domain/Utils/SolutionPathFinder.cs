namespace FhirArtifactAnalyzer.Domain.Utils
{
    public static class SolutionPathFinder
    {
        public static string GetSolutionDirectory()
        {
            var currentDir = AppContext.BaseDirectory;

            while (Directory.GetFiles(currentDir, "*.sln").Length == 0)
            {
                currentDir = Directory.GetParent(currentDir)?.FullName
                    ?? throw new DirectoryNotFoundException("Solution directory not found.");
            }

            return currentDir;
        }
    }
}