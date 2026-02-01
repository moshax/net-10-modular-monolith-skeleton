using FluentValidation;
using Inventory.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class InventoryModuleRegistration
{
    public static IServiceCollection AddInventoryModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInventoryApplication();
        services.AddInventoryInfrastructure(configuration);
        return services;
    }

    public static IServiceCollection AddInventoryApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateProductUseCase>();
        services.AddScoped<GetProductUseCase>();
        services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();
        return services;
    }

    public static IServiceCollection AddInventoryInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryUnitOfWork, InventoryUnitOfWork>();
        return services;
    }
}
