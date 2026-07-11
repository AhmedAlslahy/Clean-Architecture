using FluentValidation;

namespace Application.Features.Authentication.Login;

public class LoginDtoValidator : AbstractValidator<LoginCommand>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
       .NotEmpty()
       .WithMessage("Email is required.")
       .EmailAddress()
       .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");
    }
}