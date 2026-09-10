using Domain.Common;

namespace Domain.ValueObjects;

public sealed class PersonName : ValueObject
{
    public string First { get; }
    public string Last { get; }

    private PersonName(string first, string last)
    {
        First = first;
        Last = last;
    }

    public static PersonName Create(string first, string last)
    {
        if (string.IsNullOrWhiteSpace(first))
            throw new DomainException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(last))
            throw new DomainException("Last name cannot be empty.");
        if (first.Length > 100 || last.Length > 100)
            throw new DomainException("Name fields cannot exceed 100 characters.");

        return new PersonName(first.Trim(), last.Trim());
    }

    public string FullName => $"{First} {Last}";

    public override string ToString() => FullName;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return First;
        yield return Last;
    }
}
