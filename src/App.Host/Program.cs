using BuildingBlocks;
using Identity.Api;
using Identity.Infrastructure;
using Inventory.Api;
using Inventory.Infrastructure;
using Sales.Api;
using Sales.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddGlobalExceptionHandling();

builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddSalesModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();
app.UseCors("Frontend");

var apiV1 = app.MapGroup("/api/v1");
apiV1.MapIdentityEndpoints();
apiV1.MapSalesEndpoints();
apiV1.MapInventoryEndpoints();

app.Run();

public partial class Program;
