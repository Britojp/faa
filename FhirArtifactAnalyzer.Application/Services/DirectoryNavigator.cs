using FhirArtifactAnalyzer.Domain.Abstractions;
using Microsoft.VisualBasic.FileIO;

namespace FhirArtifactAnalyzer.Application.Services
{
    /// <summary>
    /// Responsável pela navegação em diretórios, fazendo varreduras para a busca de arquivos relevantes.
    /// </summary>
    public class DirectoryNavigator : IDirectoryNavigator
    {
        public IEnumerable<string> GetJsonFiles(string root, int maxDepth = 5, int maxFiles = 1000)
        {
            var collected = new List<string>();
            TraverseDirectory(root, 0, maxDepth, maxFiles, collected);
            return collected;
        }

        /// <summary>
        /// Navega no diretório e subdiretórios recursivamente, adicionando arquivos a uma lista,
        /// e obedecendo aos limites de profundidade e quantidade de arquivos.
        /// </summary>
        private void TraverseDirectory(string targetDirectory, int currentDepth, int maxDepth, int maxFiles, List<string> collected)
        {
            if (currentDepth >= maxDepth || collected.Count >= maxFiles)
                return;

            string[] files = Directory.GetFiles(targetDirectory);

            foreach (var file in files)
            {
                if (collected.Count >= maxFiles)
                    break;

                if (ShouldIgnoreFile(file))
                    continue;

                if (IsJsonFile(file))
                    collected.Add(file);
            }

            string[] subdirectories = Directory.GetDirectories(targetDirectory);

            foreach (var subdirectory in subdirectories)
            {
                if (collected.Count >= maxFiles)
                    break;

                if (ShouldIgnoreDirectory(subdirectory))
                    continue;

                TraverseDirectory(subdirectory, currentDepth + 1, maxDepth, maxFiles, collected);
            }
        }

        static private bool IsJsonFile(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase);
        }

        static private bool ShouldIgnoreDirectory(string directoryPath)
        {
            var directoriesToIgnore = new[] { ".git", "node_modules" };
            var directoryName = Path.GetFileName(directoryPath);

            return directoriesToIgnore
                .Any(ignored => string.Equals(ignored, directoryName, StringComparison.OrdinalIgnoreCase));
        }

        static private bool ShouldIgnoreFile(string filePath)
        {
            var filesToIgnore = new[] { ".exe", ".md", ".xml" };
            var fileName = Path.GetFileName(filePath);

            return filesToIgnore
                .Any(ignored => string.Equals(ignored, fileName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
