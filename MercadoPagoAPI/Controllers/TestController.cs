using MercadoPagoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Resend;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MercadoPagoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IEmailService emailService, IResend _resend,IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        // GET: api/<EmailController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            await emailService.SendEmailAsync("alvaroku123@gmail.com", "Test Resend", "Test Body");
            return new List<string>() { "success"};
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            return Ok(new { env = webHostEnvironment.EnvironmentName, status = "Healthy" });
        }
    }
}
