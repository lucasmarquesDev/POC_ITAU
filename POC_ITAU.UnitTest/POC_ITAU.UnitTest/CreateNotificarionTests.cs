using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using POC_ITAU.Application.UseCases.CreateNotificarion;
using POC_ITAU.Domain.Entities;
using POC_ITAU.Domain.Interfaces;
using Xunit;

namespace POC_ITAU.IntegrationTest
{
    public class CreateNotificarionTests
    {
        [Fact]
        public async Task Handle_ShouldProduceMessageToKafka_WhenSNSIsAvailable()
        {
            // Arrange
            var awsSettings = new AWSSettings
            {
                AccessKey = "ABC123",
                SecretKey = "123ABC",
                Region = "us-east-1",
                TopicArn = "topic-sns"
            };

            var mockSettings = new Mock<IOptions<AWSSettings>>();
            mockSettings.Setup(x => x.Value).Returns(awsSettings);

            var mockSNSService = new Mock<ISNSService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<CreateNotificarionHandler>>();

            mockSettings.Object.Value.AccessKey = "ABC123";
            mockSettings.Object.Value.SecretKey = "123ABC";
            mockSettings.Object.Value.Region = "us-east-1";
            mockSettings.Object.Value.TopicArn = "topic-sns";

            var handler = new CreateNotificarionHandler(mockSNSService.Object, mockMapper.Object, mockSettings.Object, mockLogger.Object);

            var request = new CreateNotificarionRequest("lunatec09@gmail.com", "POC_ENTREVISTA", "Entrevista");

            var mappedRequest = new CreateNotificarionRequest("lunatec09@gmail.com", "Test Subject", "Test Message");

            mockMapper.Setup(m => m.Map<CreateNotificarionRequest>(request)).Returns(mappedRequest);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            mockSNSService.Verify(k => k.ProduceAsync(mockSettings.Object.Value.TopicArn, mappedRequest), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Handle_ShouldRetryAndFallback_WhenSNSFails()
        {
            // Arrange
            var awsSettings = new AWSSettings
            {
                AccessKey = "ABC123",
                SecretKey = "123ABC",
                Region = "us-east-1",
                TopicArn = "topic-sns"
            };

            var mockSettings = new Mock<IOptions<AWSSettings>>();
            mockSettings.Setup(x => x.Value).Returns(awsSettings);

            var mockSNSService = new Mock<ISNSService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<CreateNotificarionHandler>>();

            var handler = new CreateNotificarionHandler(mockSNSService.Object, mockMapper.Object, mockSettings.Object, mockLogger.Object);

            var request = new CreateNotificarionRequest("lunatec09@gmail.com", "POC_ENTREVISTA", "Entrevista");

            var mappedRequest = new CreateNotificarionRequest("lunatec09@gmail.com", "Test Subject", "Test Message");

            mockMapper.Setup(m => m.Map<CreateNotificarionRequest>(request)).Returns(mappedRequest);

            //falha
            mockSNSService
                .Setup(k => k.ProduceAsync(mockSettings.Object.Value.TopicArn, mappedRequest))
                .ThrowsAsync(new Exception("Erro ao publicar no SNS"));

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            mockSNSService.Verify(k => k.ProduceAsync(mockSettings.Object.Value.TopicArn, mappedRequest), Times.Exactly(4));
            Assert.NotNull(result);
        }
    }
}
