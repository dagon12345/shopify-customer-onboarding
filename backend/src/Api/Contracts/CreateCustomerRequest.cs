namespace Api.Contracts;

public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    bool AcceptsMarketing,
    string? Note);
