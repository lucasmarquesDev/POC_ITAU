using AutoMapper;
using Confluent.Kafka;
using MediatR;
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

            var handler = new CreateNotificarionHandler(mockKafkaService.Object, mockMapper.Object);

            var request = new CreateNotificarionRequest("test@example.com", "Test Subject", "Test Message");

            var mappedRequest = new CreateNotificarionRequest("test@example.com", "Test Subject", "Test Message");

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

            var handler = new CreateNotificarionHandler(mockKafkaService.Object, mockMapper.Object);

            var request = new CreateNotificarionRequest("test@example.com", "Test Subject", "Test Message");

            var mappedRequest = new CreateNotificarionRequest("test@example.com", "Test Subject", "Test Message");

            mockMapper.Setup(m => m.Map<CreateNotificarionRequest>(request)).Returns(mappedRequest);

            // Simula uma falha no Kafka
            mockKafkaService
                .Setup(k => k.ProduceAsync("email-notifications", mappedRequest))
                .ThrowsAsync(new KafkaException(ErrorCode.BrokerNotAvailable));

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            mockKafkaService.Verify(k => k.ProduceAsync("email-notifications", mappedRequest), Times.Exactly(3));
            Assert.NotNull(result);
        }
    }
}
