using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces;
using Domain.Customers;
using Infrastructure.Shopify.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Shopify;

public sealed class ShopifyCustomerGateway : IShopifyCustomerGateway
{
    public const string HttpClientName = "ShopifyAdminApi";

    private readonly HttpClient _httpClient;
    private readonly ILogger<ShopifyCustomerGateway> _logger;

    public ShopifyCustomerGateway(HttpClient httpClient, ILogger<ShopifyCustomerGateway> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ShopifyGatewayResult> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken)
    {
        var payload = new ShopifyCustomerRequestEnvelope
        {
            Customer = new ShopifyCustomerRequest
            {
                FirstName = customer.Name.First,
                LastName = customer.Name.Last,
                Email = customer.Email.Value,
                Phone = customer.Phone?.Value,
                Note = customer.Note,
                EmailMarketingConsent = new ShopifyMarketingConsent
                {
                    State = customer.AcceptsMarketing ? "subscribed" : "not_subscribed"
                }
            }
        };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("customers.json", payload, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ShopifyCustomerResponseEnvelope>(cancellationToken);
                if (result?.Customer is null)
                    return ShopifyGatewayResult.Failed("Shopify returned an unexpected response shape.");

                return ShopifyGatewayResult.Succeeded(result.Customer.Id.ToString());
            }

            var errorMessage = await ExtractErrorMessageAsync(response, cancellationToken);
            _logger.LogWarning(
                "Shopify customer creation failed with status {StatusCode}: {Error}",
                (int)response.StatusCode,
                errorMessage);

            return ShopifyGatewayResult.Failed(errorMessage);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error communicating with Shopify Admin API for customer {CustomerId}", customer.Id);
            return ShopifyGatewayResult.Failed("Could not reach Shopify. Please try again shortly.");
        }
    }

    private static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var body = await response.Content.ReadFromJsonAsync<ShopifyErrorResponse>(cancellationToken);
            if (body is null) return $"Shopify request failed ({(int)response.StatusCode}).";

            return body.Errors.ValueKind switch
            {
                JsonValueKind.String => body.Errors.GetString() ?? "Shopify rejected the request.",
                JsonValueKind.Object => string.Join("; ", FlattenObjectErrors(body.Errors)),
                _ => $"Shopify request failed ({(int)response.StatusCode})."
            };
        }
        catch (JsonException)
        {
            return $"Shopify request failed ({(int)response.StatusCode}).";
        }
    }

    private static IEnumerable<string> FlattenObjectErrors(JsonElement errorsObject)
    {
        foreach (var field in errorsObject.EnumerateObject())
        {
            var messages = field.Value.ValueKind == JsonValueKind.Array
                ? string.Join(", ", field.Value.EnumerateArray().Select(v => v.ToString()))
                : field.Value.ToString();

            yield return $"{field.Name}: {messages}";
        }
    }
}
