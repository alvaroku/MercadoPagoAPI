using MercadoPagoAPI.Models.DTOs;
using MercadoPagoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
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
        private readonly IConfiguration _configuration; 
        public CheckoutController(ICheckoutService paymentService, IEmailService emailService, ILogger<CheckoutController> logger, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
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
            // 1. Obtener el header de firma
            var xSignature = Request.Headers["x-signature"].ToString();
            var xRequestId = Request.Headers["x-request-id"].ToString(); // Opcional para logs

            if (string.IsNullOrEmpty(xSignature))
            {
                _logger.LogWarning("Firma de webhook ausente. RequestId: {Id}", xRequestId);
                return Unauthorized("Firma ausente");
            }

            try
            {
                // 2. Validar la firma
                if (!IsValidSignature(xSignature, json))
                {
                    _logger.LogWarning("Firma de webhook inválida. RequestId: {Id}", xRequestId);
                    return Unauthorized("Firma inválida");
                }

                // 3. Procesar si es auténtico
                await _paymentService.ProcessWebhookAsync(json);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing webhook");
                return Ok(); // Siempre Ok para evitar que MP reintente ante errores lógicos
            }
        }

        private bool IsValidSignature(string signatureHeader, JsonElement body)
        {
            // Mercado Pago usa el ID del evento y el timestamp para firmar.
            // La firma viene como: ts=TIMESTAMP,v1=HASH
            var parts = signatureHeader.Split(',');
            if (parts.Length < 2) return false;

            var ts = parts[0].Split('=')[1];
            var v1 = parts[1].Split('=')[1];

            var secret = _configuration["MercadoPago:WebhookSecret"];

            // El mensaje a validar se construye con el ID del recurso y el timestamp 
            // pero para Webhooks V2 simplificados, a veces es el cuerpo crudo.
            // Consulta tu versión, pero el estándar HMAC es:
            var manifest = $"id:{body.GetProperty("data").GetProperty("id").GetString()};request-id:{Request.Headers["x-request-id"]};ts:{ts};";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(manifest));
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString == v1;
        }
    }
}
