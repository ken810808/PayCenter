using System.Net;

namespace PayCenter.MiddleWares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
           

            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authorization) ||
               authorization.First() != "token_123")
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                httpContext.Response.ContentType = "text/plain";
                //await httpContext.Response.WriteAsync("token验证失败");
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
