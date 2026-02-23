using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks;

public sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    // LoggerMessage delegates for performance (CA1848)
    private static readonly Action<ILogger, string, PathString, Exception?> _requestCancelled =
        LoggerMessage.Define<string, PathString>(
            LogLevel.Warning,
            new EventId(1001, nameof(ExceptionHandlingMiddleware)),
            "Request was cancelled by the client: {Method} {Path}");



    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Validate parameters up front (CA1062)
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        try
        {
            await next(context).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Use the precompiled delegate
            _requestCancelled(logger, context.Request.Method, context.Request.Path, null);
            context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest; // Non-standard status code for client cancellation
        }

    }
}

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
    {
        return services.AddTransient<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
