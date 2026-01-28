using MercadoPagoAPI.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MercadoPagoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IEmailService emailService,IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        // GET: api/<EmailController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            await emailService.SendEmailAsync("alvaroku123@gmail.com", "Test", "Test Body");
            return new List<string>();
        }

        // GET api/<EmailController>/5
        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok(new { status = "Healthy", env = webHostEnvironment.EnvironmentName });
        }

        // POST api/<EmailController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EmailController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmailController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
