using Microsoft.EntityFrameworkCore;
using Payment.API.Entities;

namespace Payment.API.Data;

public class PaymentContext : DbContext
{
    public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
    {
    }

    public DbSet<Entities.Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Payment Configuration
        modelBuilder.Entity<Entities.Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.HasIndex(e => e.PaymentNumber).IsUnique();
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.UserName);
            entity.HasIndex(e => e.PaymentDate);
            entity.HasIndex(e => e.Status);
            
            entity.Property(e => e.PaymentNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentMethod).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();
            
            // Customer Information
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CustomerPhone).IsRequired().HasMaxLength(20);
            
            // Payment Details
            entity.Property(e => e.CardName).HasMaxLength(100);
            entity.Property(e => e.CardLastFourDigits).HasMaxLength(4);
            entity.Property(e => e.TransactionId).HasMaxLength(50);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.GatewayTransactionId).HasMaxLength(100);
            entity.Property(e => e.BankName).HasMaxLength(50);
            entity.Property(e => e.BankNamePersian).HasMaxLength(50);
            
            // Additional Information
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DescriptionPersian).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.NotesPersian).HasMaxLength(1000);
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.FailureReasonPersian).HasMaxLength(500);
            
            // Audit Fields
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(100);
        });
    }
}