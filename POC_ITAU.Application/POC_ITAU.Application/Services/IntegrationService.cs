using MediatR;
using POC_ITAU.Application.UseCases.CreateNotificarion;
using POC_ITAU.Domain.Entities.Request;
using POC_ITAU.Domain.Interfaces;

namespace POC_ITAU.Application.Services
{
    public class IntegrationService : IIntergrationService
    {
        private readonly IMediator _mediator;

        public IntegrationService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendNotifcation(Notification notification)
        {
            await _mediator.Send(new CreateNotificarionRequest(notification.Destination, notification.Subject, notification.Message));
        }
    }
}
