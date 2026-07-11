using Domain.Common;
using MediatR;

namespace Application.Features.Authentication.SendForgetPasswordOTP;

public sealed record SendForgetPasswordOTPCommand(
string Email
) : IRequest<Result>;