using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace POC_ITAU.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers.ContainsKey("X-Correlation-ID")
                ? context.Request.Headers["X-Correlation-ID"].ToString()
                : Guid.NewGuid().ToString();

            var activity = Activity.Current ?? new Activity("Incoming Request");
            activity.SetTag("correlation_id_app", correlationId);
            activity.Start();

            context.Response.Headers["X-Correlation-ID"] = correlationId;

            _logger.LogInformation("** Correlation ID: {CorrelationId}, Trace ID: {TraceId}", correlationId, activity.TraceId);

            using (_logger.BeginScope("{CorrelationId}", correlationId))
            {
                await _next(context);
            }

            activity.Stop();
        }
    }
}
