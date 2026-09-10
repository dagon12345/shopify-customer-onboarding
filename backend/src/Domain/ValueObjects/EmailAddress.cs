using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.ValueObjects;

public sealed partial class EmailAddress : ValueObject
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email address cannot be empty.");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > 254 || !EmailRegex().IsMatch(normalized))
            throw new DomainException($"'{value}' is not a valid email address.");

        return new EmailAddress(normalized);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
