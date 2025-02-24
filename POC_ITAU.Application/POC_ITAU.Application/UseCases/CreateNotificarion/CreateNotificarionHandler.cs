using AutoMapper;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using POC_ITAU.Domain.Interfaces;
using Polly;
using Polly.Wrap;

namespace POC_ITAU.Application.UseCases.CreateNotificarion
{
    public class CreateNotificarionHandler : IRequestHandler<CreateNotificarionRequest, CreateNotificarionResponse>
    {
        private readonly IKafkaService _kafkaService;
        private readonly IMapper _mapper;
        private readonly AsyncPolicyWrap _policy;
        private readonly ILogger<CreateNotificarionHandler> _logger;

        public CreateNotificarionHandler(IKafkaService kafkaService, IMapper mapper, ILogger<CreateNotificarionHandler> logger)
        {
            _kafkaService = kafkaService;
            _mapper = mapper;
            _logger = logger;

            var circuitBreakerPolicy = Policy
                .Handle<KafkaException>()
                .Or<TimeoutException>()
                .CircuitBreakerAsync(4, TimeSpan.FromMinutes(1),
                    onBreak: (exception, duration) =>
                    {
                        _logger.LogCritical($"** Circuito ABERTO devido a falhas repetidas no Kafka. Tentando novamente em {duration.TotalSeconds}s");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("** Circuito FECHADO: Kafka voltou a responder.");
                    });

            var retryPolicy = Policy
                .Handle<KafkaException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"** [Tentativa {retryCount}] Kafka falhou, tentando novamente em {timeSpan.TotalSeconds}s. Erro: {exception.Message}", ConsoleColor.Red);
                    });

            var fallbackPolicy = Policy
                .Handle<KafkaException>()
                .Or<TimeoutException>()
                .FallbackAsync(async (cancellationToken) =>
                {
                    _logger.LogError("** Kafka indisponível! Salvando mensagem para reprocessamento...");
                    await SaveMessageForLaterAsync();
                });

            _policy = Policy.WrapAsync(fallbackPolicy, retryPolicy, circuitBreakerPolicy);
        }

        public async Task<CreateNotificarionResponse> Handle(CreateNotificarionRequest request, CancellationToken cancellationToken)
        {
            var notificationMap = _mapper.Map<CreateNotificarionRequest>(request);

            try
            {
                await _policy.ExecuteAsync(async () =>
                {
                    await _kafkaService.ProduceAsync("email-notifications", notificationMap);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"** Erro ao processar a notificação: {ex.Message}");
            }

            return new CreateNotificarionResponse();
        }

        private async Task SaveMessageForLaterAsync()
        {
            await Task.Delay(500);
            _logger.LogWarning("** Mensagem salva para reprocessamento futuro.");
        }
    }
}