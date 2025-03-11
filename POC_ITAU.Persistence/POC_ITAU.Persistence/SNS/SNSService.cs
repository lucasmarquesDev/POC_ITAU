using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using POC_ITAU.Domain.Entities;
using POC_ITAU.Domain.Interfaces;
using System.Text.Json;

namespace POC_ITAU.Persistence.SNS
{
    public class SNSService : ISNSService
    {
        private readonly IAmazonSimpleNotificationService _client;
        private readonly ILogger<SNSService> _logger;

        public SNSService(IAmazonSimpleNotificationService client, ILogger<SNSService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task ProduceAsync<T>(string topicArn, T notification)
        {
            var message = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(notification)
            };

            var jsonMessage = JsonSerializer.Serialize(message);

            var request = new PublishRequest
            {
                //TopicArn = topicArn,
                TopicArn = "arn:aws:sns:us-east-1:577618662404:poc_itau_topic_sns",
                Message = jsonMessage
            };

            var response = await _client.PublishAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation($"Mensagem publicada no SNS com sucesso: {response.MessageId}");
            }
            else
            {
                _logger.LogError($"Erro ao publicar no SNS: {response.HttpStatusCode}");
                throw new Exception("Erro ao publicar no SNS");
            }
        }
    }
}