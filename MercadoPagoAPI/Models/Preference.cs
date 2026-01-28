namespace MercadoPagoAPI.Models
{
    public class Preference
    {
        public Guid Id { get; set; }
        public string ExternalReference { get; set; } = string.Empty;
        public string MpPreferenceId { get; set; } = string.Empty;
        public string? InitPoint { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "pending"; // pending, approved, rejected
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación: Una preferencia puede resultar en un pago final
        public virtual Payment? Payment { get; set; }
    }
}
