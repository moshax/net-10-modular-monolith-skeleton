using Inventory.Application;
using Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure;

public sealed class ProductRepository(InventoryDbContext dbContext) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken).ConfigureAwait(true);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Add(product);
        await Task.CompletedTask.ConfigureAwait(true);
    }
}

public sealed class InventoryUnitOfWork(InventoryDbContext dbContext) : IInventoryUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
