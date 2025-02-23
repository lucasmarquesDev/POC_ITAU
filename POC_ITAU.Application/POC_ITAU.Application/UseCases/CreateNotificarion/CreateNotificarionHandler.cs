using AutoMapper;
using Confluent.Kafka;
using MediatR;
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

        public CreateNotificarionHandler(IKafkaService kafkaService, IMapper mapper)
        {
            _kafkaService = kafkaService;
            _mapper = mapper;

            var circuitBreakerPolicy = Policy
                .Handle<KafkaException>()
                .Or<TimeoutException>()
                .CircuitBreakerAsync(3, TimeSpan.FromMinutes(1),
                    onBreak: (exception, duration) =>
                    {
                        Console.WriteLine($"** Circuito ABERTO devido a falhas repetidas no Kafka. Tentando novamente em {duration.TotalSeconds}s");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("** Circuito FECHADO: Kafka voltou a responder.");
                    });

            var retryPolicy = Policy
                .Handle<KafkaException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"** [Tentativa {retryCount}] Kafka falhou, tentando novamente em {timeSpan.TotalSeconds}s. Erro: {exception.Message}", ConsoleColor.Red);
                    });

            var fallbackPolicy = Policy
                .Handle<KafkaException>()
                .Or<TimeoutException>()
                .FallbackAsync(async (cancellationToken) =>
                {
                    Console.WriteLine("** Kafka indisponível! Salvando mensagem para reprocessamento...");
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
                Console.WriteLine($"** Erro ao processar a notificação: {ex.Message}");
            }

            return new CreateNotificarionResponse();
        }

        private async Task SaveMessageForLaterAsync()
        {
            // Simulação de salvar no banco de dados para reprocessar depois
            await Task.Delay(500);
            Console.WriteLine("** Mensagem salva para reprocessamento futuro.");
        }
    }
}