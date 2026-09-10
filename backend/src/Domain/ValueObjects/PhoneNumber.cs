using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.ValueObjects;

public sealed partial class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Phone number cannot be empty.");

        var normalized = value.Trim();

        if (!PhoneRegex().IsMatch(normalized))
            throw new DomainException($"'{value}' is not a valid phone number.");

        return new PhoneNumber(normalized);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^\+?[0-9()\-.\s]{7,20}$")]
    private static partial Regex PhoneRegex();
}
