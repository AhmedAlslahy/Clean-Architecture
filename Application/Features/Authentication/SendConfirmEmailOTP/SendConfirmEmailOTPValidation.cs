using FluentValidation;

namespace Application.Features.Authentication.SendConfirmEmailOTP;

public class SendConfirmEmailOTPValidation : AbstractValidator<SendConfirmEmailOTPCommand>
{
    public SendConfirmEmailOTPValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}