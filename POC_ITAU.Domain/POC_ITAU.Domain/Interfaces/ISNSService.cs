namespace POC_ITAU.Domain.Interfaces
{
    public interface ISNSService
    {
        Task ProduceAsync<T>(T notification);
    }
}
