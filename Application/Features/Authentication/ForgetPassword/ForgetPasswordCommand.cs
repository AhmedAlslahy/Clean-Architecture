using Domain.Common;
using MediatR;

namespace Application.Features.Authentication.ForgetPassword;

public sealed record ForgetPasswordCommand(
string Email,
string OTP,
string NewPassword
) : IRequest<Result>;