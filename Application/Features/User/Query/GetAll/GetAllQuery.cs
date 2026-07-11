using Domain.Common;
using MediatR;

namespace Application.Features.User.Query.GetAll;

public sealed record GetAllQuery(
    ) : IRequest<Result<List<GetAllResponse>>>;