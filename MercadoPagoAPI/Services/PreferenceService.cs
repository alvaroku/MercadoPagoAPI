using MercadoPagoAPI.Models;
using MercadoPagoAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MercadoPagoAPI.Services
{
    public interface IPreferenceService
    {
        Task CreateAsync(CheckoutRequest checkoutRequest, CheckoutResponse checkoutResponse);
        Task<Preference?> GetByExternalReferenceAsync(string externalReference);
    }
    public class PreferenceService : IPreferenceService
    {
        private readonly AppDbContext _context;
        public PreferenceService(AppDbContext context) => _context = context;

        public async Task CreateAsync(CheckoutRequest checkoutRequest, CheckoutResponse checkoutResponse)
        {
            Preference preference = new Preference
            {
                ExternalReference = checkoutRequest.ExternalReference,
                TotalAmount = checkoutRequest.GetAmount(),
                InitPoint = checkoutResponse.InitPoint,
                MpPreferenceId = checkoutResponse.PreferenceId!,
            };

            _context.Preferences.Add(preference);
            await _context.SaveChangesAsync();
        }

        public async Task<Preference?> GetByExternalReferenceAsync(string externalReference)
        {
            return await _context.Preferences
                .FirstOrDefaultAsync(p => p.ExternalReference == externalReference);
        }
    }
}
