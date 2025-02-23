using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using POC_ITAU.API.Middlewares;
using POC_ITAU.Application.Services;
using POC_ITAU.Domain.Interfaces;
using POC_ITAU.Persistence.Kafka;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

string datadogApiKey = "e9b7e7461742585d9859e20368b009f6";  // 🔥 Insira sua API Key do Datadog
string datadogEndpoint = "https://otel.datadoghq.com"; // Endpoint oficial da Datadog para OpenTelemetry

Log.Logger = new LoggerConfiguration()
    .WriteTo.DatadogLogs(
        "https://http-intake.logs.datadoghq.com",
        datadogApiKey,
        "poc-itau", // Nome da fonte do log
        "poc-itau-service" // Nome do serviço
    )
    .CreateLogger();

// Configuração do OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("POC_ITAU")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("POC_ITAU"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(datadogEndpoint);
                options.Headers = $"api-key={datadogApiKey}";
            });
    })
    .WithMetrics(metricsProviderBuilder =>
    {
        metricsProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(datadogEndpoint);
                options.Headers = $"api-key={datadogApiKey}";
            });
    });

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.ParseStateValues = true;

    options.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("https://otlp.logs.datadoghq.com");
        otlpOptions.Headers = $"api-key={datadogApiKey}";
    });
});

builder.Services.ConfigureApplicationApp();

// Add services to the container.
builder.Services.AddTransient<IIntergrationService, IntegrationService>();

builder.Services.AddSingleton(provider =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:Kafka_URI"],
        MessageTimeoutMs = 5000,
        RequestTimeoutMs = 5000,
        SocketTimeoutMs = 5000
    };

    return new ProducerBuilder<string, string>(config).Build();
});

builder.Services.AddSingleton<IKafkaService, KafkaService>();

builder.Services.AddHealthChecks();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health-check");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
