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
        public CheckoutController(ICheckoutService paymentService, IEmailService emailService)
        {
            _paymentService = paymentService;
            _emailService = emailService;
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
            try
            {
                await _paymentService.ProcessWebhookAsync(json);
            }
            catch (Exception e)
            {
                await _emailService.SendEmailAsync("alvaro.ku.dev@gmail.com", "Webhook Processing Error", e.Message+"- json: "+json.GetString());
            }
            return Ok();
        }
    }
}
