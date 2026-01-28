namespace MercadoPagoAPI.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string MpPaymentId { get; set; } = string.Empty; // ID que viene de MP
        public string PaymentMethod { get; set; } = string.Empty; // credit_card, cash, etc.
        public decimal AmountPaid { get; set; }
        public DateTime DateApproved { get; set; }

        // Relación con la preferencia
        public Guid PreferenceId { get; set; }
        public virtual Preference Preference { get; set; } = null!;
    }
}
