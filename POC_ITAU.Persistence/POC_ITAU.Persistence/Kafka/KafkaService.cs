using Confluent.Kafka;
using POC_ITAU.Domain.Entities.Request;
using POC_ITAU.Domain.Interfaces;
using System.Text.Json;

namespace POC_ITAU.Persistence.Kafka
{
    public class KafkaService : IKafkaService
    {
        private readonly IProducer<string, string> _producer;

        public KafkaService(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public async Task ProduceAsync(string topic, object notification)
        {
            var message = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(notification)
            };

            await _producer.ProduceAsync(topic, message);
        }
    }
}
