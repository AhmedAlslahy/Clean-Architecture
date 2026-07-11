using Domain.Common;
using MediatR;

namespace Application.Features.Authentication.Login;

public sealed record LoginCommand(
string Email,
string Password
) : IRequest<Result<LoginResponse>>;