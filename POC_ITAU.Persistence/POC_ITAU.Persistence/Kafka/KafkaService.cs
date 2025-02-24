using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using POC_ITAU.Domain.Interfaces;
using System.Text.Json;

namespace POC_ITAU.Persistence.Kafka
{
    public class KafkaService : IKafkaService
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaService> _logger;

        public KafkaService(IProducer<string, string> producer, ILogger<KafkaService> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task ProduceAsync<T>(string topic, T notification)
        {
            var message = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(notification)
            };

            var deliveryResult = await _producer.ProduceAsync(topic, message);

            if (deliveryResult.Status == PersistenceStatus.Persisted)
            {
                _logger.LogInformation("Notificação Persistida");
            }
            else
            {
                throw new KafkaException(ErrorCode.BrokerNotAvailable);
            }
        }
    }
}