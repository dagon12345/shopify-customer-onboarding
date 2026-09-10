namespace Api.Contracts;

public sealed record CreateCustomerResponse(
    Guid CustomerId,
    bool ShopifySyncSucceeded,
    string? ShopifyCustomerId,
    string? ShopifySyncError);
