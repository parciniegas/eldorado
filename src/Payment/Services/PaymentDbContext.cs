using Microsoft.EntityFrameworkCore;
using Pay = Payment.Model.Payment;

namespace Payment.Services;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Pay> Payments { get; set; } // Using Pay as defined in IPaymentServices

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pay>().ToTable("Payments");
        // Add any additional configurations for the Payment entity if needed
        // For example, if 'Pay' has more properties or relationships.
        // modelBuilder.Entity<Pay>().HasKey(p => p.Id); // Assuming Id is the primary key
    }
}
