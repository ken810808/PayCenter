using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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
            string requestBody;

            httpContext.Request.EnableBuffering();
            using (var bodyReader = new StreamReader(stream: httpContext.Request.Body,
                                                     encoding: Encoding.UTF8,
                                                     detectEncodingFromByteOrderMarks: false,
                                                     bufferSize: 1024,
                                                     leaveOpen: true))
            {
                requestBody = await bodyReader.ReadToEndAsync();
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            _logger.LogDebug($"RequestPath: {httpContext.Request.Path}, RequestBody: {JsonSerializer.Serialize(requestBody, _options)}");

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
