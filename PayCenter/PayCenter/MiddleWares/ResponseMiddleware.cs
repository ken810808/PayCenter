using System.Text.Json;

namespace PayCenter.MiddleWares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseMiddleware> _logger;
        private readonly JsonSerializerOptions _options;

        public ResponseMiddleware(RequestDelegate next, ILogger<ResponseMiddleware> logger, JsonSerializerOptions options)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string responseContent;

            var originalBodyStream = httpContext.Response.Body;
            using (var fakeResponseBody = new MemoryStream())
            {
                httpContext.Response.Body = fakeResponseBody;

                await _next(httpContext);

                fakeResponseBody.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(fakeResponseBody))
                {
                    responseContent = await reader.ReadToEndAsync();
                    fakeResponseBody.Seek(0, SeekOrigin.Begin);

                    await fakeResponseBody.CopyToAsync(originalBodyStream);
                }
            }

            _logger.LogDebug($"ResponseStatusCode: {httpContext.Response.StatusCode}, ResponseBody: {JsonSerializer.Serialize(responseContent, _options)}");
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseMiddleware>();
        }
    }
}
