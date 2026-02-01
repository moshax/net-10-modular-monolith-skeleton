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

builder.Services.AddGlobalExceptionHandling();

builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddSalesModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandling();

app.MapIdentityEndpoints();
app.MapSalesEndpoints();
app.MapInventoryEndpoints();

app.Run();

public partial class Program;
