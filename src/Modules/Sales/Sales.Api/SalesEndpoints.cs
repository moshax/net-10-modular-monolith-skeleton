using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sales.Application;

namespace Sales.Api;

public static class SalesEndpoints
{
    public static IEndpointRouteBuilder MapSalesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/sales/orders");

        group.MapPost("", async (CreateOrderRequest request, CreateOrderUseCase useCase, CancellationToken cancellationToken) =>
        {
            var command = new CreateOrderCommand(request.OrderNo, request.CustomerName, request.Total, request.Status, request.CustomerId);
            var result = await useCase.HandleAsync(command, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/api/v1/sales/orders/{result.Value?.Id}", result.Value)
                : Results.BadRequest(new { error = result.Error });
        });

        group.MapGet("{id:guid}", async (Guid id, GetOrderUseCase useCase, CancellationToken cancellationToken) =>
        {
            var result = await useCase.HandleAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { error = result.Error });
        });

        return endpoints;
    }
}

public sealed record CreateOrderRequest(string OrderNo, string CustomerName, decimal Total, string Status, Guid? CustomerId);
