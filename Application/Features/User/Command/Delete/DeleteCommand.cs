using Domain.Common;
using MediatR;

namespace Application.Features.User.Command.Delete;

public sealed record DeleteCommand(
    string UserId
) : IRequest<Result>;