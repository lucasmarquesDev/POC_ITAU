using POC_ITAU.API.Middlewares;
using POC_ITAU.API.Extensions;
using POC_ITAU.Application.Services;
using POC_ITAU.Application.Extensions;
using POC_ITAU.Domain.Interfaces;
using POC_ITAU.Persistence.SNS;
using Serilog;
using Amazon.SimpleNotificationService;
using POC_ITAU.Domain.Entities;
using Amazon.Runtime;
using Amazon;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog(builder.Configuration, "API Observability");
Log.Information("Starting API");

builder.Services.AddElasticsearch(builder.Configuration);

builder.Services.ConfigureApplicationApp();

builder.Services.AddTransient<IIntergrationService, IntegrationService>();

var awsSettings = builder.Configuration.GetSection("AWS").Get<AWSSettings>();

builder.Services.AddSingleton(awsSettings);

var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");

var credentials = new BasicAWSCredentials(accessKey, secretKey);
var snsClient = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.GetBySystemName(awsSettings.Region));

builder.Services.AddSingleton<IAmazonSimpleNotificationService>(snsClient);
builder.Services.AddSingleton<ISNSService, SNSService>();


builder.Services.AddHealthChecks();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilog();

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