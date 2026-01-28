using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPagoAPI.Models.DTOs;
using System.Text.Json;

namespace MercadoPagoAPI.Services
{
    public interface ICheckoutService
    {
        Task<CheckoutResponse> CreatePreferenceAsync(CheckoutRequest checkoutRequest);
        Task ProcessWebhookAsync(JsonElement json);
    }

    public class MercadoPagoService : ICheckoutService
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IPaymentService _paymentService;
        private readonly IPreferenceService _preferenceService;
        private readonly IEmailService _emailService;
        public MercadoPagoService(IConfiguration config, IHostEnvironment hostEnvironment, IPaymentService paymentService, IPreferenceService preferenceService, IEmailService emailService)
        {
            _config = config;
            _hostEnvironment = hostEnvironment;
            _paymentService = paymentService;
            _preferenceService = preferenceService;
            _emailService = emailService;
            MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];
        }

        public async Task<CheckoutResponse> CreatePreferenceAsync(CheckoutRequest checkoutRequest)
        {
            var client = new PreferenceClient();
            var request = new PreferenceRequest
            {
                Items = checkoutRequest.Products.Select(p => new PreferenceItemRequest
                {
                    Title = p.Title,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    CurrencyId = "MXN"
                }).ToList(),
                ExternalReference = checkoutRequest.ExternalReference, // Vínculo clave
                BackUrls = new()
                {
                    Success = _config["MercadoPago:UrlSuccess"], // URL de retorno
                    Failure = _config["MercadoPago:UrlFailure"],
                    Pending = _config["MercadoPago:UrlPending"]
                },
                AutoReturn = "approved"
            };

            var preference = await client.CreateAsync(request);
          
            var response = new CheckoutResponse
            {
                PreferenceId = preference.Id,
                InitPoint = _hostEnvironment.IsProduction() ? preference.InitPoint : preference.SandboxInitPoint,
            };

            await _preferenceService.CreateAsync(checkoutRequest, response);
            
            return response;
        }

        public async Task ProcessWebhookAsync(JsonElement json)
        {
            try
            {
                await _emailService.SendEmailAsync("alvaro.ku.dev@gmail.com","Mercado Pago Webhook", json.GetString()??"");
            }
            catch (Exception)
            {

            }
            if (json.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var idProp))
            {
                var paymentId = idProp.GetString();
                var client = new PaymentClient();
                var payment = await client.GetAsync(long.Parse(paymentId!));

                await _paymentService.ProcessPaymentNotificationAsync(payment.ExternalReference, payment.TransactionAmount!.Value, payment!.Id!.ToString()!, payment.PaymentMethodId,payment.Status);
            }

        }
    }
}
