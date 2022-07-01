using System.Text;
using System.Text.Json;

namespace PayCenter.MiddleWares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestMiddleware> _logger;
        private readonly JsonSerializerOptions _options;

        public RequestMiddleware(RequestDelegate next, ILogger<RequestMiddleware> logger, JsonSerializerOptions options)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            using (var bodyReader = new StreamReader(stream: httpContext.Request.Body,
                                                     encoding: Encoding.UTF8,
                                                     detectEncodingFromByteOrderMarks: false,
                                                     bufferSize: 1024,
                                                     leaveOpen: true))
            {
                var body = await bodyReader.ReadToEndAsync();
                _logger.LogDebug($"RequestPath: {httpContext.Request.Path}, RequestBody: {body}");
            }
            httpContext.Request.Body.Position = 0;

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestMiddleware>();
        }
    }
}
