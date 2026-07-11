using Domain.Common;
using Domain.Errors;
using Domain.Identity;
using Infrastructure.Persistence;
using Infrastructure.Service.Email;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;

namespace Application.Features.Authentication.SendForgetPasswordOTP;

public class SendForgetPasswordOTPHandler(SarhneDbContext context, IEmailService emailService
    , IWebHostEnvironment env) : IRequestHandler<SendForgetPasswordOTPCommand, Result>
{
    public async Task<Result> Handle(SendForgetPasswordOTPCommand request, CancellationToken cancellation)
    {
        var user = await context.Users.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        user.OTP = otp;
        user.OTPExpire = DateTime.UtcNow.AddMinutes(5);

        await context.SaveChangesAsync();

        var path = Path.Combine(env.WebRootPath, "Templates", "ForgetPassword.html");
        var htmlBody = await File.ReadAllTextAsync(path, cancellation);
        htmlBody = htmlBody.Replace("{{Name}}", user.FullName);
        htmlBody = htmlBody.Replace("{{OTP}}", otp);
        await emailService.EmailBody(user.Email, "Forget Password", htmlBody, cancellation);

        return Result.Success();
    }
}