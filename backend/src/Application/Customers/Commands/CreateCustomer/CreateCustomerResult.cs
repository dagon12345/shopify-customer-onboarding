namespace Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerResult
{
    public required Guid CustomerId { get; init; }
    public required bool ShopifySyncSucceeded { get; init; }
    public string? ShopifyCustomerId { get; init; }
    public string? ShopifySyncError { get; init; }
}
