using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Logging;
using POC_ITAU.Domain.Entities;
using POC_ITAU.Domain.Interfaces;
using System.Text.Json;

namespace POC_ITAU.Persistence.SNS
{
    public class SNSService : ISNSService
    {
        private readonly IAmazonSimpleNotificationService _client;
        private readonly ILogger<SNSService> _logger;
        private readonly AWSSettings _awsSettings;

        public SNSService(IAmazonSimpleNotificationService client, ILogger<SNSService> logger, AWSSettings awsSettings)
        {
            _client = client;
            _logger = logger;
            _awsSettings = awsSettings;
        }

        public async Task ProduceAsync<T>(T notification)
        {
            var jsonMessage = JsonSerializer.Serialize(notification);
            var request = new PublishRequest
            {
                TopicArn = _awsSettings.TopicArn,
                Message = jsonMessage
            };

            var response = await _client.PublishAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError($"Erro ao publicar no SNS: {response.HttpStatusCode}");
                throw new Exception("Erro ao publicar no SNS");
            }

            _logger.LogInformation($"Mensagem publicada no SNS com sucesso: {response.MessageId}");
        }

    }
}