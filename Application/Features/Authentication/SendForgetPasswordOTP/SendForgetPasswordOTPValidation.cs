using FluentValidation;

namespace Application.Features.Authentication.SendForgetPasswordOTP;

public class SendForgetPasswordOTPValidation : AbstractValidator<SendForgetPasswordOTPCommand>
{
    public SendForgetPasswordOTPValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}