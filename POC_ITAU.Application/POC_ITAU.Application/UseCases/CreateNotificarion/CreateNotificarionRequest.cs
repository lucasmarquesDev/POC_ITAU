using MediatR;

namespace POC_ITAU.Application.UseCases.CreateNotificarion
{
    public class CreateNotificarionRequest : IRequest<CreateNotificarionResponse>
    {
        public string Destination { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public CreateNotificarionRequest(string destination, string subject, string message)
        {
            Destination = destination;
            Subject = subject;
            Message = message;
        }
    }
}
