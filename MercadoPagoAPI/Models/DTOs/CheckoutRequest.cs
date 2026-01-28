namespace MercadoPagoAPI.Models.DTOs
{
    public class CheckoutRequest
    {
        public string ExternalReference { get; set; } // ID interno de app
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();       
        public decimal GetAmount()
        {
            return Products.Sum(p => p.UnitPrice * p.Quantity);
        }
    }

    public class Product
    {
        public string Title { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
