using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.GlobalExceptionHandler;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, exception.Message);

        ProblemDetails problem;

        switch (exception)
        {
            case FluentValidation.ValidationException validationException:

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(
                    new ValidationProblemDetails(
                        validationException.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray()))
                    {
                        Title = "Validation Error",
                        Status = StatusCodes.Status400BadRequest
                    },
                    cancellationToken);

                return true;

            case UnauthorizedException:
                problem = new ProblemDetails
                {
                    Status = 401,
                    Title = "Unauthorized",
                    Detail = exception.Message
                };
                break;

            case ForbiddenException:
                problem = new ProblemDetails
                {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = exception.Message
                };
                break;

            case NotFoundException:
                problem = new ProblemDetails
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = exception.Message
                };
                break;

            default:
                problem = new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred."
                };
                break;
        }

        context.Response.StatusCode = problem.Status!.Value;

        await context.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}