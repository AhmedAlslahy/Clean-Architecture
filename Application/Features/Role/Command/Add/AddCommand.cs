using Domain.Common;
using MediatR;

namespace Application.Features.Role.Command.Add;

public sealed record AddCommand(
    string RoleName
) : IRequest<Result>;