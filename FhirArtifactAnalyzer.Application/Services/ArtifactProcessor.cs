using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using System.Text.RegularExpressions;

/// <summary>
/// Responsavel por receber uma entrada, direciona-la e coletar os arquivos relevantes.
/// </summary>
namespace FhirArtifactAnalyzer.Application.Services
{
    public class ArtifactProcessor : IArtifactProcessor
    {
        private readonly IDirectoryNavigator _navigator;
        private readonly IExtractorService _extractor;
        private readonly IJsonArtifactAnalyzer _analyzer;

        public ArtifactProcessor(
            IDirectoryNavigator navigator,
            IExtractorService extractor,
            IJsonArtifactAnalyzer analyzer)
        {
            _navigator = navigator;
            _extractor = extractor;
            _analyzer = analyzer;
        }

        /// <summary>
        /// Processa a entrada fornecida e retorna os artefatos FHIR validos.
        /// </summary>
        /// <param name="source">Fonte da entrada contendo caminho, tipo e filtro.</param>
        /// <returns>Lista de artefatos FHIR analisados.</returns>
        public async Task<IEnumerable<FhirArtifactInfo>> ProcessInputAsync(InputSource source)
        {
            var paths = GetPathsFromSource(source);

            if (!string.IsNullOrWhiteSpace(source.RegexFilter))
            {
                var regex = new Regex(source.RegexFilter);

                bool MatchesRegex(string path) => regex.IsMatch(Path.GetFileName(path));

                paths = paths.Where(MatchesRegex).ToList();
            }

            var analysisTasks = paths.Select(p => _analyzer.AnalyzeAsync(p, source));

            var results = await Task.WhenAll(analysisTasks);

            return results;
        }

        /// <summary>
        /// Obtem todos os caminhos de arquivos a partir da origem informada, incluindo extracao se necessario.
        /// </summary>
        /// <param name="source">Fonte da entrada.</param>
        /// <returns>Lista de caminhos de arquivos.</returns>
        private List<string> GetPathsFromSource(InputSource source)
        {
            var paths = new List<string>();

            switch (source.Type)
            {
                case InputType.Directory:
                    paths.AddRange(_navigator.GetFiles(source.PathOrUrl));
                    break;

                case InputType.SingleFile:
                    paths.Add(source.PathOrUrl);
                    break;

                case InputType.Tgz:
                case InputType.Zip:
                    {
                        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                        var extractedDir = _extractor.Extract(source.PathOrUrl, tempDir);

                        var extractedFiles = Directory.GetFiles(extractedDir, "*.*", SearchOption.AllDirectories);

                        paths.AddRange(extractedFiles);
                        break;
                    }

                case InputType.Url:
                    throw new NotImplementedException("Entrada via URL ainda nao foi implementada.");
            }

            return paths;
        }
    }
}
