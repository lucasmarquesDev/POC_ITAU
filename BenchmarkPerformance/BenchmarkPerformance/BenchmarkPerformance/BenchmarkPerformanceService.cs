using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using POC_ITAU.Application.UseCases.CreateNotificarion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkPerformance.BenchmarkPerformance
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
            _notificationRequest = new CreateNotificarionRequest("lunatec09@gmail.com", "POC_ITAU", "Entrevista");
        }

        [Benchmark]
        public async Task BenchmarkUseCaseCreateNotificarion()
        {
            await _mediator.Send(_notificationRequest);
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateNotificarionHandler).Assembly));
            services.AddAutoMapper(typeof(CreateNotificarionMapper).Assembly);

            return services;
        }
    }
}
