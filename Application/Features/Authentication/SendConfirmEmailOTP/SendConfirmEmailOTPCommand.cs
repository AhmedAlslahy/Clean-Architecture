using Domain.Common;
using MediatR;

namespace Application.Features.Authentication.SendConfirmEmailOTP;

public sealed record SendConfirmEmailOTPCommand(
string Email
) : IRequest<Result>;