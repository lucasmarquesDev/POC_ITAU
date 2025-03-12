using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Elastic.CommonSchema;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POC_ITAU.Application.UseCases.CreateNotificarion;
using POC_ITAU.BenchmarkPerformance;
using POC_ITAU.Domain.Entities;
using POC_ITAU.Domain.Interfaces;
using POC_ITAU.Persistence.SNS;

namespace POC_ITAU.BenchmarkPerformance
{
    [RankColumn]
    [MemoryDiagnoser]
    public class BenchmarkPerformanceService
    {
        private readonly ISender _mediator;
        private CreateNotificarionRequest _notificationRequest;

        public BenchmarkPerformanceService()
        {
            var services = ConfigureServices();
            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<ISender>();
        }

        [GlobalSetup]
        public void Setup()
        {
            _notificationRequest = new CreateNotificarionRequest("marques.nogueira@live.com", "POC_ITAU", "Entrevista");
        }

        [Benchmark]
        public async Task BenchmarkUseCaseCreateNotificarion()
        {
            await _mediator.Send(_notificationRequest);
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("C:\\Users\\marqu\\source\\repos\\POC_ITAU\\POC_ITAU.API\\appsettings.Development.json", optional: false, reloadOnChange: true)
                .Build();

            var awsSettings = configuration.GetSection("AWS").Get<AWSSettings>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateNotificarionHandler).Assembly));
            services.AddScoped<ISNSService, SNSService>();

            services.AddSingleton(awsSettings);

            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY", EnvironmentVariableTarget.Machine);
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY", EnvironmentVariableTarget.Machine);

            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var snsClient = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.GetBySystemName(awsSettings.Region));

            services.AddSingleton<IAmazonSimpleNotificationService>(snsClient);

            services.AddLogging();

            return services;
        }

    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BenchmarkPerformanceService>();
    }
}