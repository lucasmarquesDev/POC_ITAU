using Microsoft.Extensions.Logging;
using Moq;
using POC_ITAU.Application.UseCases.CreateNotificarion;
using POC_ITAU.Domain.Interfaces;
using Xunit;

namespace POC_ITAU.IntegrationTest
{
    public class CreateNotificarionTests
    {
        private readonly Mock<ILogger<CreateNotificarionHandler>> _mockLogger;
        private readonly Mock<ISNSService> _mockSNSService;
        private readonly CreateNotificarionHandler _handler;

        public CreateNotificarionTests()
        {
            _mockSNSService = new Mock<ISNSService>();
            _mockLogger = new Mock<ILogger<CreateNotificarionHandler>>();

            _handler = new CreateNotificarionHandler(_mockSNSService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldProduceMessageToKafka_WhenSNSIsAvailable()
        {
            var request = new CreateNotificarionRequest("marques.nogueira@live.com", "POC_ENTREVISTA", "Entrevista");

            _mockSNSService.Setup(k => k.ProduceAsync(request)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(request, CancellationToken.None);

            _mockSNSService.Verify(s => s.ProduceAsync(It.IsAny<CreateNotificarionRequest>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldRetryAndFallback_WhenSNSFails()
        {
            var request = new CreateNotificarionRequest("marques.nogueira@live.com", "POC_ENTREVISTA", "Entrevista");

            _mockSNSService
                .Setup(k => k.ProduceAsync(request))
                .ThrowsAsync(new Exception("Erro ao publicar no SNS"));

            var result = await _handler.Handle(request, CancellationToken.None);

            _mockSNSService.Verify(k => k.ProduceAsync(request), Times.Exactly(4));
        }
    }
}
