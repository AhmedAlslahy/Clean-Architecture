using Domain.Common;
using MediatR;

namespace Application.Features.User.Command.AddAdminRole;

public sealed record AddAdminRoleCommand(
    string UserId
) : IRequest<Result>;