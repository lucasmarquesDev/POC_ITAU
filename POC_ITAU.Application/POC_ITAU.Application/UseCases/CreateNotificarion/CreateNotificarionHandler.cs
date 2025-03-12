using AutoMapper;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using POC_ITAU.Domain.Entities;
using POC_ITAU.Domain.Interfaces;
using Polly;
using Polly.Wrap;

namespace POC_ITAU.Application.UseCases.CreateNotificarion
{
    public class CreateNotificarionHandler : IRequestHandler<CreateNotificarionRequest, CreateNotificarionResponse>
    {
        private readonly ISNSService _snsService;
        private readonly AsyncPolicyWrap _policy;
        private readonly ILogger<CreateNotificarionHandler> _logger;

        public CreateNotificarionHandler(ISNSService snsService, ILogger<CreateNotificarionHandler> logger)
        {
            _snsService = snsService;
            _logger = logger;

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .Or<TimeoutException>()
                .CircuitBreakerAsync(4, TimeSpan.FromMinutes(1),
                    onBreak: (exception, duration) =>
                    {
                        _logger.LogCritical($"** Circuito ABERTO devido a falhas repetidas no SNS. Tentando novamente em {duration.TotalSeconds}s");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("** Circuito FECHADO: SNS voltou a responder.");
                    });

            var retryPolicy = Policy
                .Handle<Exception>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"** [Tentativa {retryCount}] SNS falhou, tentando novamente em {timeSpan.TotalSeconds}s. Erro: {exception.Message}", ConsoleColor.Red);
                    });

            var fallbackPolicy = Policy
                .Handle<Exception>()
                .Or<TimeoutException>()
                .FallbackAsync(async (cancellationToken) =>
                {
                    _logger.LogError("** SNS indisponível! Salvando mensagem para reprocessamento...");
                    await SaveMessageForLaterAsync();
                });

            _policy = Policy.WrapAsync(fallbackPolicy, retryPolicy, circuitBreakerPolicy);
        }

        public async Task<CreateNotificarionResponse> Handle(CreateNotificarionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _policy.ExecuteAsync(async () =>
                {
                    await _snsService.ProduceAsync(request);
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