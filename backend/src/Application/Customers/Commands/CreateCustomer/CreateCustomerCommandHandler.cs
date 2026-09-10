using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Messaging;
using Domain.Customers;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IShopifyCustomerGateway _shopifyGateway;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IShopifyCustomerGateway shopifyGateway,
        IUnitOfWork unitOfWork,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _shopifyGateway = shopifyGateway;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Email);

        if (await _customerRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new ConflictException($"A customer with email '{email}' already exists.");

        var customer = Customer.Create(
            PersonName.Create(request.FirstName, request.LastName),
            email,
            string.IsNullOrWhiteSpace(request.Phone) ? null : PhoneNumber.Create(request.Phone),
            request.AcceptsMarketing,
            request.Note);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var gatewayResult = await _shopifyGateway.CreateCustomerAsync(customer, cancellationToken);

        if (gatewayResult.Success)
        {
            customer.MarkAsSynced(gatewayResult.ShopifyCustomerId!);
        }
        else
        {
            _logger.LogWarning(
                "Shopify sync failed for customer {CustomerId}: {Reason}",
                customer.Id,
                gatewayResult.ErrorMessage);
            customer.MarkSyncFailed(gatewayResult.ErrorMessage ?? "Unknown Shopify error.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCustomerResult
        {
            CustomerId = customer.Id,
            ShopifySyncSucceeded = gatewayResult.Success,
            ShopifyCustomerId = gatewayResult.ShopifyCustomerId,
            ShopifySyncError = gatewayResult.ErrorMessage
        };
    }
}
