using Inventory.Domain;

namespace Inventory.Application;

public sealed record ProductDto(Guid Id, string Sku, string Name, int QtyOnHand, DateTimeOffset CreatedAt);

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
}

public interface IInventoryUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
