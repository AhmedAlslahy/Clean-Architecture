using Domain.Common;
using MediatR;

namespace Application.Features.UserSetting.Query.GetByUserId;

public record GetByUserIdQuery
    : IRequest<Result<GetByUserIdResponse>>;