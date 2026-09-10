using Domain.Customers;

namespace Application.Common.Interfaces;

/// <summary>
/// Outbound port to the Shopify Admin API. Implemented in Infrastructure so the
/// Application layer stays free of any Shopify-specific transport concerns.
/// </summary>
public interface IShopifyCustomerGateway
{
    Task<ShopifyGatewayResult> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken);
}

public sealed record ShopifyGatewayResult
{
    public bool Success { get; init; }
    public string? ShopifyCustomerId { get; init; }
    public string? ErrorMessage { get; init; }

    public static ShopifyGatewayResult Succeeded(string shopifyCustomerId) =>
        new() { Success = true, ShopifyCustomerId = shopifyCustomerId };

    public static ShopifyGatewayResult Failed(string errorMessage) =>
        new() { Success = false, ErrorMessage = errorMessage };
}
