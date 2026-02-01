using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application;

namespace Sales.Infrastructure;

public static class SalesModuleRegistration
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSalesApplication();
        services.AddSalesInfrastructure(configuration);
        return services;
    }

    public static IServiceCollection AddSalesApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<GetOrderUseCase>();
        services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();
        return services;
    }

    public static IServiceCollection AddSalesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SalesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISalesUnitOfWork, SalesUnitOfWork>();
        return services;
    }
}
