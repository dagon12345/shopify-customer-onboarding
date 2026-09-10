using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Customers;

public sealed class Customer : Entity
{
    public PersonName Name { get; private set; } = null!;
    public EmailAddress Email { get; private set; } = null!;
    public PhoneNumber? Phone { get; private set; }
    public bool AcceptsMarketing { get; private set; }
    public string? Note { get; private set; }

    public CustomerSyncStatus SyncStatus { get; private set; }
    public string? ShopifyCustomerId { get; private set; }
    public string? SyncFailureReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? SyncedAtUtc { get; private set; }

    private Customer()
    {
        // Reserved for EF Core materialization.
    }

    public static Customer Create(
        PersonName name,
        EmailAddress email,
        PhoneNumber? phone,
        bool acceptsMarketing,
        string? note)
    {
        if (note is { Length: > 1000 })
            throw new DomainException("Note cannot exceed 1000 characters.");

        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Phone = phone,
            AcceptsMarketing = acceptsMarketing,
            Note = note,
            SyncStatus = CustomerSyncStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void MarkAsSynced(string shopifyCustomerId)
    {
        if (string.IsNullOrWhiteSpace(shopifyCustomerId))
            throw new DomainException("Shopify customer id cannot be empty when marking as synced.");

        SyncStatus = CustomerSyncStatus.Synced;
        ShopifyCustomerId = shopifyCustomerId;
        SyncFailureReason = null;
        SyncedAtUtc = DateTime.UtcNow;
    }

    public void MarkSyncFailed(string reason)
    {
        SyncStatus = CustomerSyncStatus.Failed;
        SyncFailureReason = reason;
    }
}
