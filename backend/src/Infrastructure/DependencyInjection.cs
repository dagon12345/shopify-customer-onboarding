using Application.Common.Interfaces;
using Domain.Customers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Shopify;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<ShopifyOptions>(configuration.GetSection(ShopifyOptions.SectionName));

        var shopifyOptions = configuration.GetSection(ShopifyOptions.SectionName).Get<ShopifyOptions>()
                              ?? new ShopifyOptions();

        services.AddHttpClient<IShopifyCustomerGateway, ShopifyCustomerGateway>(client =>
            {
                client.BaseAddress = new Uri($"https://{shopifyOptions.ShopDomain}/admin/api/{shopifyOptions.ApiVersion}/");
                client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", shopifyOptions.AccessToken);
                client.Timeout = TimeSpan.FromSeconds(15);
            })
            .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => (int)msg.StatusCode == 429)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    }
}
