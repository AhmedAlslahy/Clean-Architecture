using Application.Common.Abstraction;
using Domain.Enums;

namespace Application.Common.Errors;

public class AuthErrors
{
    public static readonly Error NotFound
     = new("Auth.NotFound", "Auth not found", ErrorType.NotFound);

    public static readonly Error InvalidData
       = new("Auth.InvalidData", "Invalid Auth data", ErrorType.BadRequest);

    public static readonly Error InvalidCredentials
     = new("Auth.InvalidCredentials", "Invalid Email Or Password", ErrorType.BadRequest);

    public static readonly Error InvalidRefreshToken =
    new("Auth.InvalidRefreshToken", "Invalid or expired refresh token", ErrorType.Unauthorized);
}