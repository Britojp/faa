using FhirArtifactAnalyzer.Domain.Abstractions;
using Moq;

namespace FhirArtifactAnalyzer.Tests.UnitTests.ServicesUnitTests
{
    /// <summary>
    /// Testes unitarios para o componente que orquestra a extracao de entradas,
    /// utilizando manipuladores especificos para cada tipo.
    /// </summary>
    public class ExtractorServiceUnitTests
    {
        /// <summary>
        /// Deve executar a extracao corretamente quando ha um manipulador que aceita o caminho.
        /// </summary>
        [Fact]
        public void Extract_ShouldUseCorrectHandler_WhenHandlerCanHandlePath()
        {
            var path = "input.tgz";
            var destination = "output/";
            var expectedResult = "output/ok";

            var mockHandler = new Mock<IInputHandler>();
            mockHandler.Setup(h => h.CanHandle(path)).Returns(true);
            mockHandler.Setup(h => h.Extract(path, destination)).Returns(expectedResult);

            var service = CreateServiceWithHandlers(mockHandler.Object);

            var result = service.Extract(path, destination);

            Assert.Equal(expectedResult, result);
            mockHandler.Verify(h => h.CanHandle(path), Times.Once);
            mockHandler.Verify(h => h.Extract(path, destination), Times.Once);
        }

        /// <summary>
        /// Deve lançar excecao quando nenhum manipulador conseguir lidar com o caminho fornecido.
        /// </summary>
        [Fact]
        public void Extract_ShouldThrowNotSupportedException_WhenNoHandlerCanHandle()
        {
            var path = "input.unsupported";
            var destination = "output/";

            var mockHandler = new Mock<IInputHandler>();
            mockHandler.Setup(h => h.CanHandle(It.IsAny<string>())).Returns(false);

            var service = CreateServiceWithHandlers(mockHandler.Object);

            var exception = Assert.Throws<NotSupportedException>(() => service.Extract(path, destination));

            Assert.Contains("Tipo de arquivo nao suportado", exception.Message);
            mockHandler.Verify(h => h.CanHandle(path), Times.Once);
            mockHandler.Verify(h => h.Extract(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Deve utilizar o primeiro manipulador que conseguir lidar com o caminho,
        /// mesmo que haja outros disponiveis na lista.
        /// </summary>
        [Fact]
        public void Extract_ShouldUseFirstMatchingHandler_WhenMultipleHandlersExist()
        {
            var path = "input.tgz";
            var destination = "dest/";
            var expectedResult = "dest/tgz";

            var mockHandler1 = new Mock<IInputHandler>();
            var mockHandler2 = new Mock<IInputHandler>();

            mockHandler1.Setup(h => h.CanHandle(path)).Returns(false);
            mockHandler2.Setup(h => h.CanHandle(path)).Returns(true);
            mockHandler2.Setup(h => h.Extract(path, destination)).Returns(expectedResult);

            var service = CreateServiceWithHandlers(mockHandler1.Object, mockHandler2.Object);

            var result = service.Extract(path, destination);

            Assert.Equal(expectedResult, result);
            mockHandler1.Verify(h => h.CanHandle(path), Times.Once);
            mockHandler2.Verify(h => h.CanHandle(path), Times.Once);
            mockHandler2.Verify(h => h.Extract(path, destination), Times.Once);
        }

        /// <summary>
        /// Cria uma instancia do ExtractorService com manipuladores de entrada simulados.
        /// </summary>
        private static IExtractorService CreateServiceWithHandlers(params IInputHandler[] handlers)
        {
            return new TestableExtractorService(handlers);
        }

        /// <summary>
        /// Subclasse do servico original utilizada para injetar manipuladores simulados durante os testes.
        /// </summary>
        private class TestableExtractorService : IExtractorService
        {
            private readonly IEnumerable<IInputHandler> _handlers;

            public TestableExtractorService(IEnumerable<IInputHandler> handlers)
            {
                _handlers = handlers;
            }

            public string Extract(string path, string destinationDirectory)
            {
                var handler = _handlers.FirstOrDefault(h => h.CanHandle(path))
                    ?? throw new NotSupportedException($"Tipo de arquivo nao suportado: {path}");

                return handler.Extract(path, destinationDirectory);
            }
        }
    }
}
