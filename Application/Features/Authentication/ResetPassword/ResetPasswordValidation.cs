using FluentValidation;

namespace Application.Features.Authentication.ResetPassword;

public class ResetPasswordValidation : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidation()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters.")
            .Matches("[A-Z]")
            .WithMessage("Must contain uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Must contain lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Must contain number.");
    }
}