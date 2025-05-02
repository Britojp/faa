using FhirArtifactAnalyzer.Application.Services.Handlers;
using FhirArtifactAnalyzer.Domain.Abstractions;

namespace FhirArtifactAnalyzer.Application.Services
{
    /// <summary>
    /// Servi�o respons�vel por orquestrar a extra��o de diferentes tipos de entradas,
    /// como arquivos .tgz, .zip, utilizando os manipuladores apropriados.
    /// </summary>
    public class ExtractorService
    {
        private readonly List<IInputHandler> _handlers;

        /// <summary>
        /// Inicializa o servi�o com a lista de manipuladores de entrada dispon�veis.
        /// </summary>
        public ExtractorService()
        {
            _handlers = new List<IInputHandler>
            {
                new TgzHandler(),
                new ZipHandler(),
            };
        }

        /// <summary>
        /// Executa a extra��o do caminho especificado utilizando o manipulador apropriado, 
        /// com base no tipo de entrada.
        /// </summary>
        /// <exception cref="NotSupportedException">Lan�ada se o tipo de entrada n�o for suportado.</exception>
        public string Extract(string path, string destinationDirectory)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(path));

            if (handler == null)
                throw new NotSupportedException($"Tipo de arquivo n�o suportado: {path}");

            return handler.Extract(path, destinationDirectory);
        }
    }
}