using Domain.Customers;
using Domain.ValueObjects;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);

        builder.OwnsOne(c => c.Name, name =>
        {
            name.Property(n => n.First).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            name.Property(n => n.Last).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        });

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(254)
            .IsRequired()
            .HasConversion(email => email.Value, value => EmailAddress.Create(value));

        builder.HasIndex(c => c.Email).IsUnique();

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .HasConversion(new PhoneNumberConverter());

        builder.Property(c => c.AcceptsMarketing).HasColumnName("accepts_marketing");
        builder.Property(c => c.Note).HasColumnName("note").HasMaxLength(1000);

        builder.Property(c => c.SyncStatus).HasColumnName("sync_status").HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.ShopifyCustomerId).HasColumnName("shopify_customer_id").HasMaxLength(50);
        builder.Property(c => c.SyncFailureReason).HasColumnName("sync_failure_reason").HasMaxLength(500);

        builder.Property(c => c.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(c => c.SyncedAtUtc).HasColumnName("synced_at_utc");
    }
}
