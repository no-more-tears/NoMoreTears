using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Common.Exceptions.Integration;
using NoMoreTears.Shared.Common.Exceptions.Server;
using System.Net;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.ExceptionHandling;

/// <summary>
///     Global exception handler that captures unhandled exceptions during HTTP request processing.
/// </summary>
public sealed class GlobalExceptionHandler(
     IHostEnvironment environment,
     IProblemDetailsService detailsService,
     ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var statusCode = GetStatusCode(httpContext, exception);
        var problemDetails = CreateProblemDetails(exception, statusCode);

        logger.LogError(
            exception,
            "Exception occurred while processing the HTTP {Method} {Path} with status code {StatusCode} and message: {Message}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            (int)statusCode,
            exception.Message);

        httpContext.Response.StatusCode = (int)statusCode;

        // Write the ProblemDetails response.
        return await detailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
    }

    /// <summary>
    ///     Creates a ProblemDetails object based on the exception.
    /// </summary>
    private ProblemDetails CreateProblemDetails(
        Exception exception,
        HttpStatusCode statusCode)
    {
        string? title, detail;

        if (environment.IsStaging()) // Should be IsDevelopment() in production, but using IsStaging() for testing purposes.
        {
            title = $"$Error '{exception.GetType().FullName}' occurred with the following context: {exception.Message}";
            detail = exception.ToString();
        }
        else
        {
            (title, detail) = exception switch
            {
                ValidationException => ("Validation error", exception.Message),
                BadRequestException or ExternalClientException => ("Bad Request", exception.Message),
                UnauthorizedException or ExternalAuthException => ("Unauthorized", exception.Message),
                ForbiddenException => ("Forbidden", exception.Message),
                NotFoundException => ("Not Found", exception.Message),
                OperationCanceledException or ExternalTimeoutException => ("Request Timeout", "The request timed out. Please try again."),
                ConflictException => ("Conflict", exception.Message),
                GoneException => ("Gone", exception.Message),
                TooManyRequestsException => ("Too Many Requests", exception.Message),
                ServiceUnavailableException or ExternalUnavailableException => ("Service Unavailable", "The service is temporarily unavailable. Please try again later."),
                _ => ("Internal Server Error", "An unexpected error occurred. Please contact support if the problem persists.")
            };
        }

        return new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Type = GetErrorType((int)statusCode),
            Detail = detail
        };
    }

    /// <summary>
    ///     Determines the appropriate HTTP status code based on the exception type and request path.
    /// </summary>
    private static HttpStatusCode GetStatusCode(HttpContext httpContext, Exception exception)
    {
        if (httpContext.Request.Path.HasValue && httpContext.Request.Path.Value.Contains("token/refresh"))
        {
            return HttpStatusCode.Unauthorized;
        }

        return exception switch
        {
            ValidationException or BadRequestException or ExternalClientException => HttpStatusCode.BadRequest,
            UnauthorizedException or ExternalAuthException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            NotFoundException => HttpStatusCode.NotFound,
            OperationCanceledException or ExternalTimeoutException => HttpStatusCode.RequestTimeout,
            ConflictException => HttpStatusCode.Conflict,
            GoneException => HttpStatusCode.Gone,
            TooManyRequestsException => HttpStatusCode.TooManyRequests,
            ServiceUnavailableException or ExternalUnavailableException => HttpStatusCode.ServiceUnavailable,
            _ => HttpStatusCode.InternalServerError
        };
    }

    /// <summary>
    ///     Gets the error type URL based on the status code.
    /// </summary>
    private static string GetErrorType(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        StatusCodes.Status401Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
        StatusCodes.Status403Forbidden => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
        StatusCodes.Status404NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
        StatusCodes.Status408RequestTimeout => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.7",
        StatusCodes.Status409Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
        StatusCodes.Status410Gone => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.9",
        StatusCodes.Status429TooManyRequests => "https://datatracker.ietf.org/doc/html/rfc6585#section-4",
        StatusCodes.Status503ServiceUnavailable => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.4",
        _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
    };
}