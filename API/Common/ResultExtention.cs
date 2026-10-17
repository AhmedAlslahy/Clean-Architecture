using Application.Common.Abstraction;

namespace API.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        if (!result.IsSuccess)
            return Results.BadRequest(result.Failure);

        return Results.Ok(result.IsSuccess);
    }

    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (!result.IsSuccess)
            return Results.BadRequest(result.Failure);

        return Results.Ok(result.Data);
    }
}