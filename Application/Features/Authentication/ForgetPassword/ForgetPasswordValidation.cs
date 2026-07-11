using FluentValidation;

namespace Application.Features.Authentication.ForgetPassword;

public class ForgetPasswordValidation : AbstractValidator<ForgetPasswordCommand>
{
    public ForgetPasswordValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.OTP)
            .NotEmpty()
            .WithMessage("OTP is required.")
            .Matches(@"^\d{6}$");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");
    }
}