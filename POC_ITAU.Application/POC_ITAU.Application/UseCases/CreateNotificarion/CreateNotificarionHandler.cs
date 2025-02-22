using AutoMapper;
using MediatR;
using POC_ITAU.Domain.Interfaces;

namespace POC_ITAU.Application.UseCases.CreateNotificarion
{
    public class CreateNotificarionHandler : IRequestHandler<CreateNotificarionRequest, CreateNotificarionResponse>
    {
        private readonly IKafkaService _kafkaService;
        private readonly IMapper _mapper;

        public CreateNotificarionHandler(IKafkaService kafkaService, IMapper mapper)
        {
            _kafkaService = kafkaService;
            _mapper = mapper;
        }

        public async Task<CreateNotificarionResponse> Handle(CreateNotificarionRequest request, CancellationToken cancellationToken)
        {
            var notificationMap = _mapper.Map<CreateNotificarionRequest>(request);

            await _kafkaService.ProduceAsync("notifications-topic", notificationMap);

            return new CreateNotificarionResponse();
        }
    }
}
