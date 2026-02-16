using Microsoft.AspNetCore.Builder;

namespace Presentation.Extensions
{
    // Convenience extension to register the exception handling middleware in Program.cs
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Presentation.Middleware.ExceptionHandlingMiddleware>();
        }
    }
}
