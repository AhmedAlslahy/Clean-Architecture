using Domain.Common;
using MediatR;

namespace Application.Features.Authentication.ResetPassword;

public sealed record ResetPasswordCommand(
 string CurrentPassword,
 string NewPassword
) : IRequest<Result>;