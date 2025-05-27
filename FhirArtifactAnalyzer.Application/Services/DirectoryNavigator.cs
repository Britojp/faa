using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Constants;

namespace FhirArtifactAnalyzer.Application.Services
{
    /// <summary>
    /// Responsável pela navegação em diretórios, fazendo varreduras para a busca de arquivos.
    /// </summary>
    public class DirectoryNavigator : IDirectoryNavigator
    {
        public IEnumerable<string> GetFiles(string root, int maxDepth = 5, int maxFiles = 1000)
        {
            var collected = new List<string>();
            TraverseDirectory(root, 0, maxDepth, maxFiles, collected);
            return collected;
        }

        /// <summary>
        /// Navega no diretório e subdiretórios recursivamente, adicionando arquivos a uma lista,
        /// e obedecendo aos limites de profundidade e quantidade de arquivos.
        /// </summary>
        private static void TraverseDirectory(
            string targetDirectory, 
            int currentDepth, 
            int maxDepth, 
            int maxFiles, 
            List<string> collected)
        {
            if (currentDepth >= maxDepth || collected.Count >= maxFiles)
                return;

            var files = Directory.GetFiles(targetDirectory);

            foreach (var file in files)
            {
                if (collected.Count >= maxFiles)
                    break;

                if (ShouldIgnoreFile(file))
                    continue;

                collected.Add(file);
            }

            var subdirectories = Directory.GetDirectories(targetDirectory);

            foreach (var subdirectory in subdirectories)
            {
                if (collected.Count >= maxFiles)
                    break;

                if (ShouldIgnoreDirectory(subdirectory))
                    continue;

                TraverseDirectory(subdirectory, currentDepth + 1, maxDepth, maxFiles, collected);
            }
        }

        private static bool ShouldIgnoreDirectory(string directoryPath)
        {
            var directoriesToIgnore = new[] { ".git", "node_modules" };
            var directoryName = Path.GetFileName(directoryPath);

            return directoriesToIgnore
                .Any(ignoredDirectory => string.Equals(ignoredDirectory, directoryName, StringComparison.OrdinalIgnoreCase));
        }

        private static bool ShouldIgnoreFile(string filePath)
        {
            var filesToIgnore = new[] 
            { 
                FileExtensions.Exe, 
                FileExtensions.Md , 
                FileExtensions.Xml 
            };

            var fileExtension = Path.GetExtension(filePath);

            return filesToIgnore
                .Any(ignoredExtension => string.Equals(ignoredExtension, fileExtension, StringComparison.OrdinalIgnoreCase));
        }
    }
}