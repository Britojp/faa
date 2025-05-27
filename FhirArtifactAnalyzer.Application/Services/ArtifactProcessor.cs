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
        private readonly DirectoryNavigator _navigator = new();
        private readonly ExtractorService _extractor = new();
        private readonly JsonArtifactAnalyzer _analyzer = new();

        /// <summary>
        /// Processa a entrada fornecida e retorna os artefatos FHIR validos.
        /// </summary>
        /// <param name="source">Fonte da entrada contendo caminho, tipo e filtro.</param>
        /// <returns>Lista de artefatos FHIR analisados.</returns>
        public async Task<IList<FhirArtifactInfo>> ProcessInputAsync(InputSource source)
        {
            var paths = GetPathsFromSource(source);

            if (!string.IsNullOrWhiteSpace(source.RegexFilter))
            {
                var regex = new Regex(source.RegexFilter);
                paths = paths.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();
            }

            var results = await Task.WhenAll(paths.Select(p =>
            {
                var info = new FhirArtifactInfo
                {
                    FilePath = p,
                    Source = source
                };
                return _analyzer.AnalyzeAsync(info);
            }));

            return results.ToList();
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
                    var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    var extractedDir = _extractor.Extract(source.PathOrUrl, tempDir);
                    paths.AddRange(Directory.GetFiles(extractedDir, "*.*", SearchOption.AllDirectories));
                    break;

                case InputType.Url:
                    throw new NotImplementedException("Entrada via URL ainda não foi implementada.");
            }

            return paths;
        }
    }
}
