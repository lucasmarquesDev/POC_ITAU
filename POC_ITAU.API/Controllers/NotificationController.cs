using Microsoft.AspNetCore.Mvc;

namespace POC_ITAU.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(ILogger<NotificationController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "sendEmail")]
        public async Task<IActionResult> SendEmail()
        {
            return Ok();
        }
    }
}
