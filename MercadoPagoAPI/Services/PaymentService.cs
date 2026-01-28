using MercadoPagoAPI.Models;

namespace MercadoPagoAPI.Services
{
    public interface IPaymentService
    {
        Task ProcessPaymentNotificationAsync(string externalReference, decimal amount, string id, string paymentMethod, string status);
    }
    public class PaymentService : IPaymentService
    {
        private readonly IPreferenceService _preferenceService;
        private readonly AppDbContext _context;
        public PaymentService(AppDbContext context, IPreferenceService preferenceService)
        {
            _context = context;
            _preferenceService = preferenceService;
        }

        public async Task CreateAsync(Preference preference, decimal amount, string id, string paymentMethod)
        {
            preference.Payment = new Payment
            {
                AmountPaid = amount,
                MpPaymentId = id,
                PaymentMethod = paymentMethod,
                DateApproved = DateTime.UtcNow
            };
        }

        public async Task ProcessPaymentNotificationAsync(string externalReference, decimal amount, string id, string paymentMethod, string status)
        {
            Preference? preference = await _preferenceService.GetByExternalReferenceAsync(externalReference);
            if (preference == null)
                return;

            preference.Status = status;

            await CreateAsync(preference, amount, id, paymentMethod);

            await _context.SaveChangesAsync();
        }


    }
}
