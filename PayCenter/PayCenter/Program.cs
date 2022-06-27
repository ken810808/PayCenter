var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
