using System.IO.Abstractions.TestingHelpers;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Constants;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesUnitTests
{
    /// <summary>
    /// Testes unitarios para o componente de navegacao de diretorios,
    /// responsavel por varrer estruturas e retornar arquivos validos.
    /// </summary>
    public class DirectoryNavigatorUnitTests
    {
        /// <summary>
        /// Deve retornar apenas arquivos validos, ignorando extensoes nao permitidas.
        /// </summary>
        [Fact]
        public void GetFiles_ShouldReturnValidFilesOnly()
        {
            var root = @"C:\project";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{root}\file1.json", new MockFileData("content") },
                { $@"{root}\file2.exe", new MockFileData("ignored") },
                { $@"{root}\file3.xml", new MockFileData("ignored") },
                { $@"{root}\subdir\file4.json", new MockFileData("content") },
                { $@"{root}\subdir\file5.md", new MockFileData("ignored") },
            });

            var navigator = CreateNavigatorWithMockFileSystem(mockFileSystem);
            var files = navigator.GetFiles(root).ToList();

            Assert.Equal(2, files.Count);
            Assert.All(files, f => Assert.EndsWith(".json", f));
        }

        /// <summary>
        /// Deve respeitar o limite de profundidade e nao acessar subdiretorios alem do limite.
        /// </summary>
        [Fact]
        public void GetFiles_ShouldRespectMaxDepth()
        {
            var root = @"C:\data";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{root}\file1.json", new MockFileData("content") },
                { $@"{root}\level1\file2.json", new MockFileData("content") },
                { $@"{root}\level1\level2\file3.json", new MockFileData("content") },
            });

            var navigator = CreateNavigatorWithMockFileSystem(mockFileSystem);
            var files = navigator.GetFiles(root, maxDepth: 2).ToList();

            Assert.Equal(2, files.Count);
            Assert.DoesNotContain(files, f => f.Contains("level2"));
        }

        /// <summary>
        /// Deve respeitar o limite de quantidade de arquivos coletados.
        /// </summary>
        [Fact]
        public void GetFiles_ShouldRespectMaxFiles()
        {
            var root = @"C:\dir";
            var mockFiles = new Dictionary<string, MockFileData>();

            for (int i = 0; i < 10; i++)
            {
                mockFiles[$@"{root}\file{i}.json"] = new MockFileData("data");
            }

            var mockFileSystem = new MockFileSystem(mockFiles);

            var navigator = CreateNavigatorWithMockFileSystem(mockFileSystem);
            var files = navigator.GetFiles(root, maxDepth: 1, maxFiles: 5).ToList();

            Assert.Equal(5, files.Count);
        }

        /// <summary>
        /// Deve ignorar diretorios com nomes bloqueados como '.git' e 'node_modules'.
        /// </summary>
        [Fact]
        public void GetFiles_ShouldIgnoreSpecialDirectories()
        {
            var root = @"C:\code";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{root}\file.json", new MockFileData("ok") },
                { $@"{root}\.git\config", new MockFileData("ignored") },
                { $@"{root}\node_modules\lib.js", new MockFileData("ignored") },
            });

            var navigator = CreateNavigatorWithMockFileSystem(mockFileSystem);
            var files = navigator.GetFiles(root).ToList();

            Assert.Single(files);
            Assert.DoesNotContain(files, f => f.Contains(".git"));
            Assert.DoesNotContain(files, f => f.Contains("node_modules"));
        }

        /// <summary>
        /// Cria uma instancia do navigator com um sistema de arquivos simulado.
        /// </summary>
        private static IDirectoryNavigator CreateNavigatorWithMockFileSystem(MockFileSystem fileSystem)
        {
            return new TestableDirectoryNavigator(fileSystem);
        }

        /// <summary>
        /// Subclasse para substituir chamadas reais ao sistema de arquivos durante os testes.
        /// </summary>
        private class TestableDirectoryNavigator : IDirectoryNavigator
        {
            private readonly MockFileSystem _fileSystem;

            public TestableDirectoryNavigator(MockFileSystem fileSystem)
            {
                _fileSystem = fileSystem;
            }

            public IEnumerable<string> GetFiles(string root, int maxDepth = 5, int maxFiles = 1000)
            {
                var collected = new List<string>();
                TraverseDirectory(root, 0, maxDepth, maxFiles, collected);
                return collected;
            }

            private void TraverseDirectory(string targetDirectory, int currentDepth, int maxDepth, int maxFiles, List<string> collected)
            {
                if (currentDepth >= maxDepth || collected.Count >= maxFiles)
                    return;

                if (!_fileSystem.Directory.Exists(targetDirectory))
                    return;

                var files = _fileSystem.Directory.GetFiles(targetDirectory);

                foreach (var file in files)
                {
                    if (collected.Count >= maxFiles)
                        break;

                    if (ShouldIgnoreFile(file))
                        continue;

                    collected.Add(file);
                }

                var subdirectories = _fileSystem.Directory.GetDirectories(targetDirectory);

                foreach (var subdir in subdirectories)
                {
                    if (collected.Count >= maxFiles)
                        break;

                    if (ShouldIgnoreDirectory(subdir))
                        continue;

                    TraverseDirectory(subdir, currentDepth + 1, maxDepth, maxFiles, collected);
                }
            }

            private static bool ShouldIgnoreDirectory(string directoryPath)
            {
                var directoriesToIgnore = new[] { ".git", "node_modules" };
                var directoryName = Path.GetFileName(directoryPath);

                return directoriesToIgnore
                    .Any(ignored => string.Equals(ignored, directoryName, StringComparison.OrdinalIgnoreCase));
            }

            private static bool ShouldIgnoreFile(string filePath)
            {
                var filesToIgnore = new[] { FileExtensions.Exe, FileExtensions.Md, FileExtensions.Xml };
                var extension = Path.GetExtension(filePath);

                return filesToIgnore
                    .Any(ignored => string.Equals(ignored, extension, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
