namespace POC_ITAU.Domain.Interfaces
{
    public interface IKafkaService
    {
        Task ProduceAsync<T>(string topic, T notification);
    }
}
