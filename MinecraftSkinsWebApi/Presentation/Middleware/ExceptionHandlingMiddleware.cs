using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Presentation.Middleware
{
    // Centralized middleware to catch unhandled exceptions, log them and
    // produce a consistent JSON error response for API clients.
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Map common exception types to HTTP status codes. Extend as needed.
            var code = HttpStatusCode.InternalServerError;
            if (exception is ArgumentException || exception is InvalidOperationException)
                code = HttpStatusCode.BadRequest;
            else if (exception is System.Security.Authentication.AuthenticationException)
                code = HttpStatusCode.Unauthorized;

            _logger.LogError(exception, "Unhandled exception processing request {Method} {Path}", context.Request.Method, context.Request.Path);

            var result = new
            {
                error = exception.Message,
                // Include stack trace only in Development to avoid leaking details in Production
                details = _env.EnvironmentName == "Development" ? exception.StackTrace : null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            var json = JsonSerializer.Serialize(result);
            await context.Response.WriteAsync(json);
        }
    }
}
