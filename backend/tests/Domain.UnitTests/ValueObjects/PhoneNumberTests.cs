using Domain.Common;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+1 415-555-0132")]
    [InlineData("(415) 555-0132")]
    [InlineData("4155550132")]
    public void Create_WithValidValue_Succeeds(string input)
    {
        var act = () => PhoneNumber.Create(input);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("123")]
    public void Create_WithInvalidValue_ThrowsDomainException(string input)
    {
        var act = () => PhoneNumber.Create(input);

        act.Should().Throw<DomainException>();
    }
}
