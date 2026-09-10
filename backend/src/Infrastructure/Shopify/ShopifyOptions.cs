namespace Infrastructure.Shopify;

public sealed class ShopifyOptions
{
    public const string SectionName = "Shopify";

    /// <summary>e.g. "your-store.myshopify.com"</summary>
    public string ShopDomain { get; init; } = string.Empty;

    /// <summary>Admin API access token (shpat_...) from a custom app with write_customers scope.</summary>
    public string AccessToken { get; init; } = string.Empty;

    public string ApiVersion { get; init; } = "2024-10";
}
