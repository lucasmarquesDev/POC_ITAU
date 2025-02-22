using Confluent.Kafka;
using POC_ITAU.Application.Services;
using POC_ITAU.Domain.Interfaces;
using POC_ITAU.Persistence.Kafka;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApplicationApp();

// Add services to the container.
builder.Services.AddTransient<IIntergrationService, IntegrationService>();

builder.Services.AddSingleton<IProducer<string, string>>(provider =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka_URI:Kafka_URI"],
        MessageTimeoutMs = 5000,
        RequestTimeoutMs = 5000,
        SocketTimeoutMs = 5000
    };

    return new ProducerBuilder<string, string>(config).Build();
});

builder.Services.AddSingleton<IKafkaService, KafkaService>();

builder.Services.AddHealthChecks();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
