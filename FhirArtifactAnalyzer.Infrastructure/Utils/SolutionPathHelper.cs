using System.IO;

namespace FhirArtifactAnalyzer.Infrastructure.Utils
{
    public static class SolutionPathHelper
    {
        public static string GetSolutionDirectory()
        {
            var currentDir = AppContext.BaseDirectory;

            while (Directory.GetFiles(currentDir, "*.sln").Length == 0)
            {
                currentDir = Directory.GetParent(currentDir)?.FullName;
                if (currentDir == null)
                {
                    throw new DirectoryNotFoundException("Solution directory not found.");
                }
            }

            return currentDir;
        }
    }
}