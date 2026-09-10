using Domain.Customers;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.Customers;

public class CustomerTests
{
    private static Customer CreateValidCustomer() => Customer.Create(
        PersonName.Create("Jane", "Doe"),
        EmailAddress.Create("jane.doe@example.com"),
        PhoneNumber.Create("+1 415-555-0132"),
        acceptsMarketing: true,
        note: "VIP customer");

    [Fact]
    public void Create_WithValidData_StartsAsPending()
    {
        var customer = CreateValidCustomer();

        customer.SyncStatus.Should().Be(CustomerSyncStatus.Pending);
        customer.ShopifyCustomerId.Should().BeNull();
        customer.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void MarkAsSynced_SetsShopifyIdAndStatus()
    {
        var customer = CreateValidCustomer();

        customer.MarkAsSynced("123456789");

        customer.SyncStatus.Should().Be(CustomerSyncStatus.Synced);
        customer.ShopifyCustomerId.Should().Be("123456789");
        customer.SyncedAtUtc.Should().NotBeNull();
        customer.SyncFailureReason.Should().BeNull();
    }

    [Fact]
    public void MarkSyncFailed_SetsFailureReasonAndStatus()
    {
        var customer = CreateValidCustomer();

        customer.MarkSyncFailed("Email has already been taken");

        customer.SyncStatus.Should().Be(CustomerSyncStatus.Failed);
        customer.SyncFailureReason.Should().Be("Email has already been taken");
        customer.ShopifyCustomerId.Should().BeNull();
    }
}
