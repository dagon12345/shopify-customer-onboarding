using System.Text.Json.Serialization;

namespace Infrastructure.Shopify.Models;

public sealed class ShopifyCustomerRequestEnvelope
{
    [JsonPropertyName("customer")]
    public required ShopifyCustomerRequest Customer { get; init; }
}

public sealed class ShopifyCustomerRequest
{
    [JsonPropertyName("first_name")]
    public required string FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public required string LastName { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("note")]
    public string? Note { get; init; }

    [JsonPropertyName("email_marketing_consent")]
    public ShopifyMarketingConsent? EmailMarketingConsent { get; init; }
}

public sealed class ShopifyMarketingConsent
{
    [JsonPropertyName("state")]
    public required string State { get; init; }

    [JsonPropertyName("opt_in_level")]
    public string OptInLevel { get; init; } = "single_opt_in";
}
