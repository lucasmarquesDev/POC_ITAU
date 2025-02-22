using POC_ITAU.Domain.Entities.Request;

namespace POC_ITAU.Domain.Interfaces
{
    public interface IIntergrationService
    {
        Task SendNotifcation(Notification notification);
    }
}
