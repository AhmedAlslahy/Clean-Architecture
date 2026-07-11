using Domain.Common;
using MediatR;

namespace Application.Features.User.Query.GetByLink;

public sealed record GetByLinkQuery(
    string Link
) : IRequest<Result<GetByLinkResponse>>;