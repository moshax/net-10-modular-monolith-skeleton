using Identity.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/identity/users");

        group.MapPost("", async (CreateUserRequest request, CreateUserUseCase useCase, CancellationToken cancellationToken) =>
        {
            var result = await useCase.HandleAsync(new CreateUserCommand(request.Email, request.PasswordHash), cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/api/v1/identity/users/{result.Value?.Id}", result.Value)
                : Results.BadRequest(new { error = result.Error });
        });

        group.MapGet("{id:guid}", async (Guid id, GetUserUseCase useCase, CancellationToken cancellationToken) =>
        {
            var result = await useCase.HandleAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { error = result.Error });
        });

        return endpoints;
    }
}

public sealed record CreateUserRequest(string Email, string PasswordHash);
