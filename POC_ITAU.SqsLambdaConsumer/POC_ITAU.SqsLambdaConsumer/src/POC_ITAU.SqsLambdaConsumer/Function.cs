using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Text.Json;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace POC_ITAU.SqsLambdaConsumer
{
    public class Function
    {
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var record in sqsEvent.Records)
            {
                context.Logger.LogInformation($"Mensagem recebida: {record.Body}");

                // Simulação de processamento
                var data = JsonSerializer.Deserialize<dynamic>(record.Body);
                context.Logger.LogInformation($"Processando dados: {data}");
            }

            await Task.CompletedTask;
        }
    }
}