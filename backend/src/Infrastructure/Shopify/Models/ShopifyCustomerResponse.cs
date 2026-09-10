using System.Text.Json.Serialization;

namespace Infrastructure.Shopify.Models;

public sealed class ShopifyCustomerResponseEnvelope
{
    [JsonPropertyName("customer")]
    public ShopifyCustomerResponse? Customer { get; init; }
}

public sealed class ShopifyCustomerResponse
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }
}

public sealed class ShopifyErrorResponse
{
    [JsonPropertyName("errors")]
    public System.Text.Json.JsonElement Errors { get; init; }
}
