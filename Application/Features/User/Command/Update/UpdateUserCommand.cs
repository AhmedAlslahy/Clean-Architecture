using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.User.Command.Update;

public sealed record UpdateUserCommand(
    string FullName,
    string? PhoneNumber,
    string? ProfileDescription,
    IFormFile? Image,
    string? PublicLink
) : IRequest<Result>;