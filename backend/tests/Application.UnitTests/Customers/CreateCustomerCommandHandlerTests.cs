using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Customers.Commands.CreateCustomer;
using Domain.Customers;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Application.UnitTests.Customers;

public class CreateCustomerCommandHandlerTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IShopifyCustomerGateway _gateway = Substitute.For<IShopifyCustomerGateway>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _handler = new CreateCustomerCommandHandler(
            _repository,
            _gateway,
            _unitOfWork,
            Substitute.For<ILogger<CreateCustomerCommandHandler>>());
    }

    private static CreateCustomerCommand ValidCommand() => new(
        "Jane", "Doe", "jane.doe@example.com", "+1 415-555-0132", true, "VIP");

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsConflictException()
    {
        _repository.ExistsByEmailAsync(Arg.Any<EmailAddress>(), Arg.Any<CancellationToken>()).Returns(true);

        var act = async () => await _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        await _gateway.DidNotReceive().CreateCustomerAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenShopifySyncSucceeds_PersistsAndReturnsShopifyId()
    {
        _repository.ExistsByEmailAsync(Arg.Any<EmailAddress>(), Arg.Any<CancellationToken>()).Returns(false);
        _gateway.CreateCustomerAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>())
            .Returns(ShopifyGatewayResult.Succeeded("987654321"));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.ShopifySyncSucceeded.Should().BeTrue();
        result.ShopifyCustomerId.Should().Be("987654321");
        await _repository.Received(1).AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenShopifySyncFails_StillPersistsLocallyWithFailureReason()
    {
        _repository.ExistsByEmailAsync(Arg.Any<EmailAddress>(), Arg.Any<CancellationToken>()).Returns(false);
        _gateway.CreateCustomerAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>())
            .Returns(ShopifyGatewayResult.Failed("Shopify is unavailable"));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.ShopifySyncSucceeded.Should().BeFalse();
        result.ShopifySyncError.Should().Be("Shopify is unavailable");
        await _repository.Received(1).AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }
}
