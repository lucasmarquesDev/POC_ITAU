using AutoMapper;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using POC_ITAU.Application.UseCases.CreateNotificarion;
using POC_ITAU.Domain.Interfaces;
using Xunit;

namespace POC_ITAU.IntegrationTest
{
    public class CreateNotificarionTests
    {
        [Fact]
        public async Task Handle_ShouldProduceMessageToKafka_WhenKafkaIsAvailable()
        {
            // Arrange
            var mockKafkaService = new Mock<IKafkaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<CreateNotificarionHandler>>();

            var handler = new CreateNotificarionHandler(mockKafkaService.Object, mockMapper.Object, mockLogger.Object);

            var request = new CreateNotificarionRequest("lunatec09@gmail.com", "POC_ENTREVISTA", "Entrevista");

            var mappedRequest = new CreateNotificarionRequest("lunatec09@gmail.com", "Test Subject", "Test Message");

            mockMapper.Setup(m => m.Map<CreateNotificarionRequest>(request)).Returns(mappedRequest);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            mockKafkaService.Verify(k => k.ProduceAsync("email-notifications", mappedRequest), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Handle_ShouldRetryAndFallback_WhenKafkaFails()
        {
            // Arrange
            var mockKafkaService = new Mock<IKafkaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<CreateNotificarionHandler>>();

            var handler = new CreateNotificarionHandler(mockKafkaService.Object, mockMapper.Object, mockLogger.Object);

            var request = new CreateNotificarionRequest("lunatec09@gmail.com", "POC_ENTREVISTA", "Entrevista");

            var mappedRequest = new CreateNotificarionRequest("lunatec09@gmail.com", "Test Subject", "Test Message");

            mockMapper.Setup(m => m.Map<CreateNotificarionRequest>(request)).Returns(mappedRequest);

            //falha
            mockKafkaService
                .Setup(k => k.ProduceAsync("email-notifications", mappedRequest))
                .ThrowsAsync(new KafkaException(ErrorCode.BrokerNotAvailable));

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            mockKafkaService.Verify(k => k.ProduceAsync("email-notifications", mappedRequest), Times.Exactly(4));
            Assert.NotNull(result);
        }
    }
}
