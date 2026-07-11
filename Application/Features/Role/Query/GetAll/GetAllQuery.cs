using Domain.Common;
using MediatR;

namespace Application.Features.Role.Query.GetAll;

public sealed record GetAllQuery(
) : IRequest<Result<List<GetAllResponse>>>;