using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    /// <summary>
    /// Retorna os caminhos de arquivos JSON encontrados a partir de um diretório raiz, com limites de profundidade e quantidade.
    /// </summary>
    /// <param name="root">Caminho do diretório raiz.</param>
    /// <param name="maxDepth">Profundidade máxima de busca recursiva.</param>
    /// <param name="maxFiles">Número máximo de arquivos a retornar.</param>
    /// <returns>Enumerable de caminhos de arquivos JSON válidos.</returns>
    public interface IDirectoryNavigator
    {
        IEnumerable<string> GetJsonFiles(string root, int maxDepth = 5, int maxFiles = 1000);
    }
}