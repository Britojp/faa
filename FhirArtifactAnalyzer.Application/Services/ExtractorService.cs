using FhirArtifactAnalyzer.Application.Services.Handlers;
using FhirArtifactAnalyzer.Domain.Abstractions;

namespace FhirArtifactAnalyzer.Application.Services
{
    /// <summary>
    /// Serviço responsável por orquestrar a extração de diferentes tipos de entradas,
    /// como arquivos .tgz, .zip, utilizando os manipuladores apropriados.
    /// </summary>
    public class ExtractorService
    {
        private readonly List<IInputHandler> _handlers;

        /// <summary>
        /// Inicializa o serviço com a lista de manipuladores de entrada disponíveis.
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
        /// Executa a extração do caminho especificado utilizando o manipulador apropriado, 
        /// com base no tipo de entrada.
        /// </summary>
        /// <exception cref="NotSupportedException">Lançada se o tipo de entrada não for suportado.</exception>
        public string Extract(string path, string destinationDirectory)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(path));

            if (handler == null)
                throw new NotSupportedException($"Tipo de arquivo não suportado: {path}");

            return handler.Extract(path, destinationDirectory);
        }
    }
}