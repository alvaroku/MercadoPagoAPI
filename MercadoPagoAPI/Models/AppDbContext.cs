using Microsoft.EntityFrameworkCore;

namespace MercadoPagoAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
