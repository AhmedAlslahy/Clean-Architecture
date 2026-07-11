using Domain.Common;
using MediatR;

namespace Application.Features.Authentication.ConfirmEmail;

public sealed record ConfirmEmailCommand(
string OTP
) : IRequest<Result>;