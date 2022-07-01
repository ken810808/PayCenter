using NLog.Web;
using PayCenter.MiddleWares;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// NLog
//builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.Host.UseNLog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName);
});

builder.Services.AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions()
{
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    WriteIndented = false,
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseReDoc(option =>
    {
        option.RoutePrefix = "redoc";
        option.SpecUrl = $"/webapi/swagger/v1/swagger.yaml";
        option.DocumentTitle = "ReDoc接口文件";
        option.ConfigObject = new Swashbuckle.AspNetCore.ReDoc.ConfigObject
        {
            NoAutoAuth = true,
            HideDownloadButton = true,
            HideLoading = false,
            HideHostname = true,
            RequiredPropsFirst = true,
            DisableSearch = true,
            NativeScrollbars = true,
        };
    });
}

app.UseAuthorization();

app.MapControllers();

app.UseRequestMiddleware();
app.UseResponseMiddleware();

app.Run();
