using FluentValidation;

namespace Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.")
            .MaximumLength(254);

        RuleFor(c => c.Phone)
            .Matches(@"^\+?[0-9()\-.\s]{7,20}$").WithMessage("Enter a valid phone number.")
            .When(c => !string.IsNullOrWhiteSpace(c.Phone));

        RuleFor(c => c.Note)
            .MaximumLength(1000)
            .When(c => c.Note is not null);
    }
}
