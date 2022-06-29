using System.Net;
using System.Text.Json;

namespace PayCenter.MiddleWares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenAuthMiddleware> _logger;

        public TokenAuthMiddleware(RequestDelegate next, ILogger<TokenAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authorization) ||
               authorization.FirstOrDefault() != "token_123")
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                httpContext.Response.ContentType = "text/plain";
                _logger.LogDebug($"token验证失败: {authorization.FirstOrDefault() ?? "Headers without Authorization"}"); // 
                return;
            }
            await _next.Invoke(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TokenAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthMiddleware>();
        }
    }
}
