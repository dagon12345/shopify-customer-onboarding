using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Converters;

public sealed class PhoneNumberConverter : ValueConverter<PhoneNumber?, string?>
{
    public PhoneNumberConverter() : base(
        phone => phone == null ? null : phone.Value,
        value => value == null ? null : PhoneNumber.Create(value))
    {
    }
}
