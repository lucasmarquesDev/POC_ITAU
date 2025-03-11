namespace POC_ITAU.Domain.Interfaces
{
    public interface ISNSService
    {
        Task ProduceAsync<T>(string topicArn, T notification);
    }
}
