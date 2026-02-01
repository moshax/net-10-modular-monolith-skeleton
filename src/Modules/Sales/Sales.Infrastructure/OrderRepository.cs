using Microsoft.EntityFrameworkCore;
using Sales.Application;
using Sales.Domain;

namespace Sales.Infrastructure;

public sealed class OrderRepository(SalesDbContext dbContext) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Orders.Add(order);
        await Task.CompletedTask;
    }
}

public sealed class SalesUnitOfWork(SalesDbContext dbContext) : ISalesUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
