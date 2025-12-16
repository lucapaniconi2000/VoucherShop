using VoucherShop.Application.Common.Exceptions;

namespace VoucherShop.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        object payload;

        switch (exception)
        {
            case NotFoundException ex:
                statusCode = StatusCodes.Status404NotFound;
                payload = new { error = ex.Message };
                _logger.LogWarning(ex, ex.Message);
                break;

            case ValidationException ex:
                statusCode = StatusCodes.Status400BadRequest;
                payload = new { errors = ex.Errors };
                _logger.LogWarning(ex, "Validation error");
                break;

            case UnauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                payload = new { error = "Unauthorized" };
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                payload = new { error = "An unexpected error occurred" };
                _logger.LogError(exception, "Unhandled exception");
                break;
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(payload);
    }
}
