using Domain.Common;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.ValueObjects;

public class EmailAddressTests
{
    [Theory]
    [InlineData("Jane.Doe@Example.com", "jane.doe@example.com")]
    [InlineData("  user@example.com  ", "user@example.com")]
    public void Create_NormalizesToLowercaseAndTrims(string input, string expected)
    {
        var email = EmailAddress.Create(input);

        email.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    public void Create_WithInvalidValue_ThrowsDomainException(string input)
    {
        var act = () => EmailAddress.Create(input);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TwoEmailsWithSameValue_AreEqual()
    {
        var a = EmailAddress.Create("user@example.com");
        var b = EmailAddress.Create("USER@example.com");

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }
}
