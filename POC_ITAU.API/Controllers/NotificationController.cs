using Microsoft.AspNetCore.Mvc;
using POC_ITAU.Domain.Entities.Request;
using POC_ITAU.Domain.Interfaces;

namespace POC_ITAU.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly IIntergrationService _integrationService;

        public NotificationController(ILogger<NotificationController> logger, IIntergrationService intergrationService)
        {
            _logger = logger;
            _integrationService = intergrationService;
        }

        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] Notification notification)
        {
            _logger.LogInformation($"** Processando notificação para email {notification.Destination}");

            await _integrationService.SendNotifcation(notification);

            return Ok();
        }
    }
}
