using Inventory.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Inventory.Api;

public static class InventoryEndpoints
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/inventory/products");

        group.MapPost("", async (CreateProductRequest request, CreateProductUseCase useCase, CancellationToken cancellationToken) =>
        {
            var command = new CreateProductCommand(request.Sku, request.Name, request.QtyOnHand);
            var result = await useCase.HandleAsync(command, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/api/v1/inventory/products/{result.Value?.Id}", result.Value)
                : Results.BadRequest(new { error = result.Error });
        });

        group.MapGet("{id:guid}", async (Guid id, GetProductUseCase useCase, CancellationToken cancellationToken) =>
        {
            var result = await useCase.HandleAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { error = result.Error });
        });

        return endpoints;
    }
}

public sealed record CreateProductRequest(string Sku, string Name, int QtyOnHand);
