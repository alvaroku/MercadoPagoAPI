using MercadoPagoAPI.Models.DTOs;
using MercadoPagoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MercadoPagoAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _paymentService;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckoutController> _logger;
        public CheckoutController(ICheckoutService paymentService, IEmailService emailService, ILogger<CheckoutController> logger)
        {
            _paymentService = paymentService;
            _emailService = emailService;
            _logger = logger;
        }

        // Generar link de pago
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
        {
            return Ok(await _paymentService.CreatePreferenceAsync(req));
        }

        // Recibir notificaciones de Mercado Pago
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement json)
        {
            _logger.LogInformation("Received webhook: {Json}", json.GetRawText());  
            await _emailService.SendEmailAsync("alvaro.ku.dev@gmail.com", "Webhook data", "json.GetRawText()");
            try
            {
                await _paymentService.ProcessWebhookAsync(json);
            }
            catch (Exception e)
            {
                await _emailService.SendEmailAsync("alvaro.ku.dev@gmail.com", "Webhook Processing Error", e.Message + " "+ e.StackTrace);
            }
            return Ok();
        }
    }
}
