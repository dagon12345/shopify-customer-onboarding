using System.Net;
using Application.Common.Exceptions;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var problem = new ValidationProblemDetails(ex.Errors.ToDictionary(e => e.Key, e => e.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };
            await WriteProblemAsync(context, problem, StatusCodes.Status400BadRequest);
        }
        catch (ConflictException ex)
        {
            await WriteProblemAsync(context, new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = ex.Message
            }, StatusCodes.Status409Conflict);
        }
        catch (DomainException ex)
        {
            await WriteProblemAsync(context, new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid request",
                Detail = ex.Message
            }, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblemAsync(context, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
            }, StatusCodes.Status500InternalServerError);
        }
    }

    private static Task WriteProblemAsync(HttpContext context, ProblemDetails problem, int statusCode)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;
        // Serialize using the runtime type so derived properties (e.g. ValidationProblemDetails.Errors) aren't sliced off.
        return context.Response.WriteAsJsonAsync(problem, problem.GetType());
    }
}
