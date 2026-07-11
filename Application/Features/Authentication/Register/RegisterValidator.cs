using FluentValidation;

namespace Application.Features.Authentication.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(20)
            .Matches("^[a-zA-Z0-9_]+$");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password);
    }
}