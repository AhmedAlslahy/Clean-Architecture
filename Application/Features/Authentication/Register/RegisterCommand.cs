using Domain.Common;
using MediatR;

namespace Application.Features.Authentication.Register;

public sealed record RegisterCommand(
string FullName,
string UserName,
string Email,
string Password,
string ConfirmPassword
) : IRequest<Result>;