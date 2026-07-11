using Domain.Common;
using MediatR;

namespace Application.Features.Role.Command.Delete;

public sealed record DeleteCommand(
    string RoleId
) : IRequest<Result>;