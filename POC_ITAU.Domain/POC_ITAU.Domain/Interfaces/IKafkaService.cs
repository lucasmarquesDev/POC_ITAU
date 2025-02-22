namespace POC_ITAU.Domain.Interfaces
{
    public interface IKafkaService
    {
        Task ProduceAsync(string topic, object notification);
    }
}
