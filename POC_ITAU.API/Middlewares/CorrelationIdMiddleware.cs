using System.Diagnostics;

namespace POC_ITAU.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString();

            Activity.Current?.SetTag("correlation_id", correlationId);

            using (var scope = context.RequestServices.GetRequiredService<ILogger<CorrelationIdMiddleware>>()
                .BeginScope("{@CorrelationId}", correlationId))
            {
                await _next(context);
            }
        }
    }
}
