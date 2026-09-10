using Application.Common.Messaging;

namespace Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    bool AcceptsMarketing,
    string? Note) : ICommand<CreateCustomerResult>;
