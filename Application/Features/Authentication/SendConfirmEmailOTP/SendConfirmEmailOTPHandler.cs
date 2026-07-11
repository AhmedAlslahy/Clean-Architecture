using Domain.Common;
using Domain.Errors;
using Domain.Identity;
using Infrastructure.Persistence;
using Infrastructure.Service.Email;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;

namespace Application.Features.Authentication.SendConfirmEmailOTP;

public class SendConfirmEmailOTPHandler(SarhneDbContext context, IEmailService emailService
    , IWebHostEnvironment env) : IRequestHandler<SendConfirmEmailOTPCommand, Result>
{
    public async Task<Result> Handle(SendConfirmEmailOTPCommand request, CancellationToken cancellation)
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

        var path = Path.Combine(env.WebRootPath, "Templates", "EmailConfirme.html");
        var htmlBody = await File.ReadAllTextAsync(path, cancellation);
        htmlBody = htmlBody.Replace("{{Name}}", user.FullName);
        htmlBody = htmlBody.Replace("{{OTP}}", otp);
        await emailService.EmailBody(user.Email, "Confirm Email", htmlBody, cancellation);

        return Result.Success();
    }
}