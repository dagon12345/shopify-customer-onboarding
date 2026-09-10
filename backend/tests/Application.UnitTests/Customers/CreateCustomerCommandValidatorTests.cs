using Application.Customers.Commands.CreateCustomer;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Customers;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateCustomerCommand("Jane", "Doe", "jane@example.com", "+14155550132", true, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Doe", "jane@example.com")]
    [InlineData("Jane", "", "jane@example.com")]
    [InlineData("Jane", "Doe", "not-an-email")]
    [InlineData("Jane", "Doe", "")]
    public void Validate_WithInvalidRequiredFields_HasErrors(string first, string last, string email)
    {
        var command = new CreateCustomerCommand(first, last, email, null, false, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidPhone_HasError()
    {
        var command = new CreateCustomerCommand("Jane", "Doe", "jane@example.com", "abc", false, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Phone");
    }
}
